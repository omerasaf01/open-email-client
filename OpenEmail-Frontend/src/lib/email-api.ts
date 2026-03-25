import { http } from "@/lib/http";
import type {
  ApiEnvelope,
  EmailDetail,
  EmailDetailDto,
  EmailDetailResponseDto,
  EmailSummary,
  InboxEmailSummaryDto,
  InboxResponseDto,
  LoginInput,
  LoginResponse,
  SignInRequestDto,
  SignInResponseDto,
  SendEmailInput,
  SendEmailRequestDto,
} from "@/lib/types";

const endpoints = {
  login: process.env.NEXT_PUBLIC_LOGIN_ENDPOINT ?? "/auth/login",
  inbox: process.env.NEXT_PUBLIC_INBOX_ENDPOINT ?? "/inbox",
  emailById:
    process.env.NEXT_PUBLIC_EMAIL_DETAIL_ENDPOINT ?? "/emails/{Id}",
  sendEmail: process.env.NEXT_PUBLIC_SEND_EMAIL_ENDPOINT ?? "/emails",
};

function resolveEmailDetailEndpoint(id: string): string {
  const encodedId = encodeURIComponent(id);
  const template = endpoints.emailById;
  const hasPlaceholder = /\{id\}|:id/i.test(template);

  const withId = hasPlaceholder
    ? template
        .replace(/\{id\}/gi, encodedId)
        .replace(/:id/gi, encodedId)
    : `${template.replace(/\/$/, "")}/${encodedId}`;

  // The HTTP client already uses a base URL that ends with /api.
  // Strip a leading /api here to avoid accidental /api/api/... requests.
  if (withId.startsWith("/api/")) {
    return withId.replace("/api", "");
  }

  return withId;
}

function unwrap<T>(payload: T | ApiEnvelope<T>): T {
  const data = payload as ApiEnvelope<T>;

  if (data?.data !== undefined) {
    return data.data as T;
  }

  return payload as T;
}

function resolveToken(payload: SignInResponseDto | ApiEnvelope<SignInResponseDto>): string {
  const data = unwrap(payload);

  const token =
    data?.accessToken ??
    data?.token ??
    data?.jwt;

  if (!token || typeof token !== "string") {
    throw new Error("Giris yanitinda access token bulunamadi.");
  }

  return token;
}

function mapEmailSummary(item: InboxEmailSummaryDto): EmailSummary {
  return {
    id: String(item.id ?? item.emailId ?? ""),
    from: String(item.from ?? item.sender ?? item.fromAddress ?? "Unknown"),
    subject: String(item.subject ?? "(Konu yok)"),
    snippet: String(item.snippet ?? item.preview ?? item.folderOrLabel ?? ""),
    sentAt: String(
      item.sentAt ??
        item.receivedAt ??
        item.createdAt ??
        item.date ??
        new Date().toISOString(),
    ),
    isRead: Boolean(item.isRead ?? item.read ?? false),
  };
}

function normalizeRecipients(value: unknown): string {
  if (Array.isArray(value)) {
    return value.map((item) => String(item)).join(", ");
  }

  return String(value ?? "");
}

function htmlToText(value: string): string {
  return value
    .replace(/<style[\s\S]*?>[\s\S]*?<\/style>/gi, "")
    .replace(/<script[\s\S]*?>[\s\S]*?<\/script>/gi, "")
    .replace(/<[^>]+>/g, " ")
    .replace(/&nbsp;/gi, " ")
    .replace(/&amp;/gi, "&")
    .replace(/&lt;/gi, "<")
    .replace(/&gt;/gi, ">")
    .replace(/&quot;/gi, '"')
    .replace(/&#39;/gi, "'")
    .replace(/\s+/g, " ")
    .trim();
}

function mapEmailDetail(item: EmailDetailDto): EmailDetail {
  const rawBody = item.body ?? item.content;
  const fallbackBodyHtml = String(item.bodyHtml ?? "");

  return {
    id: String(item.id ?? item.emailId ?? ""),
    from: String(item.from ?? item.sender ?? item.fromAddress ?? "Unknown"),
    to: normalizeRecipients(item.to ?? item.recipient ?? item.toAddress),
    subject: String(item.subject ?? "(Konu yok)"),
    body: rawBody ? String(rawBody) : htmlToText(fallbackBodyHtml),
    sentAt: String(
      item.sentAt ??
        item.receivedAt ??
        item.createdAt ??
        item.date ??
        new Date().toISOString(),
    ),
    isRead: Boolean(item.isRead ?? item.read ?? false),
  };
}

function resolveEmailDetailPayload(payload: EmailDetailDto | EmailDetailResponseDto | ApiEnvelope<EmailDetailResponseDto>): EmailDetailDto {
  const root = unwrap(payload as EmailDetailResponseDto | ApiEnvelope<EmailDetailResponseDto>) as EmailDetailDto | EmailDetailResponseDto;

  if (root && "subject" in root) {
    return root as EmailDetailDto;
  }

  const typedRoot = root as EmailDetailResponseDto;
  const candidates = [
    typedRoot.message,
    typedRoot.emailMessage,
    typedRoot.item,
    typedRoot.email,
  ];

  for (const candidate of candidates) {
    if (candidate) {
      return candidate;
    }
  }

  return {};
}

function resolveArray(payload: InboxEmailSummaryDto[] | InboxResponseDto | ApiEnvelope<InboxResponseDto>): InboxEmailSummaryDto[] {
  if (Array.isArray(payload)) {
    return payload;
  }

  const data = unwrap(payload as InboxResponseDto | ApiEnvelope<InboxResponseDto>);
  const candidates = [
    data.emailSummaries,
    data.items,
    data.results,
    data.data,
  ];

  for (const candidate of candidates) {
    if (Array.isArray(candidate)) {
      return candidate;
    }
  }

  return [];
}

export async function login(input: LoginInput): Promise<LoginResponse> {
  const requestDto: SignInRequestDto = {
    email: input.email,
    password: input.password,
  };
  const response = await http.post<SignInResponseDto | ApiEnvelope<SignInResponseDto>>(endpoints.login, requestDto);
  const payload = unwrap(response.data);

  return {
    accessToken: resolveToken(response.data),
    user: (payload.user ?? payload.profile) as LoginResponse["user"],
  };
}

export async function getInbox(): Promise<EmailSummary[]> {
  const response = await http.get<InboxEmailSummaryDto[] | InboxResponseDto | ApiEnvelope<InboxResponseDto>>(endpoints.inbox);
  return resolveArray(response.data).map(mapEmailSummary);
}

export async function getEmailById(id: string): Promise<EmailDetail> {
  const target = resolveEmailDetailEndpoint(id);
  const response = await http.get<EmailDetailDto | EmailDetailResponseDto | ApiEnvelope<EmailDetailResponseDto>>(target);

  return mapEmailDetail(resolveEmailDetailPayload(response.data));
}

export async function sendEmail(input: SendEmailInput): Promise<void> {
  const requestDto: SendEmailRequestDto = {
    to: input.to,
    subject: input.subject,
    body: input.body,
  };
  await http.post(endpoints.sendEmail, requestDto);
}
