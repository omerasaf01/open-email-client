import { http } from "@/lib/http";
import type {
  EmailDetail,
  EmailSummary,
  LoginInput,
  LoginResponse,
  SendEmailInput,
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

function unwrap<T>(payload: unknown): T {
  const data = payload as Record<string, unknown>;

  if (data?.data) {
    return data.data as T;
  }

  return payload as T;
}

function resolveToken(payload: unknown): string {
  const data = payload as Record<string, unknown>;
  const nested = (data?.data ?? {}) as Record<string, unknown>;

  const token =
    data?.accessToken ??
    data?.token ??
    nested?.accessToken ??
    nested?.token ??
    data?.jwt;

  if (!token || typeof token !== "string") {
    throw new Error("Giris yanitinda access token bulunamadi.");
  }

  return token;
}

function mapEmailSummary(item: Record<string, unknown>): EmailSummary {
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

function mapEmailDetail(item: Record<string, unknown>): EmailDetail {
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

function resolveEmailDetailPayload(payload: unknown): Record<string, unknown> {
  const root = unwrap<Record<string, unknown>>(payload);
  const candidates = [
    root.message,
    root.emailMessage,
    root.item,
    root.email,
  ];

  for (const candidate of candidates) {
    if (candidate && typeof candidate === "object" && !Array.isArray(candidate)) {
      return candidate as Record<string, unknown>;
    }
  }

  return root;
}

function resolveArray(payload: unknown): Record<string, unknown>[] {
  if (Array.isArray(payload)) {
    return payload as Record<string, unknown>[];
  }

  const data = payload as Record<string, unknown>;
  const candidates = [
    data.emailSummaries,
    data.items,
    data.results,
    data.data,
  ];

  for (const candidate of candidates) {
    if (Array.isArray(candidate)) {
      return candidate as Record<string, unknown>[];
    }
  }

  return [];
}

export async function login(input: LoginInput): Promise<LoginResponse> {
  const response = await http.post(endpoints.login, input);
  const payload = unwrap<Record<string, unknown>>(response.data);

  return {
    accessToken: resolveToken(payload),
    user: (payload.user ?? payload.profile) as LoginResponse["user"],
  };
}

export async function getInbox(): Promise<EmailSummary[]> {
  const response = await http.get(endpoints.inbox);
  return resolveArray(response.data).map(mapEmailSummary);
}

export async function getEmailById(id: string): Promise<EmailDetail> {
  const target = resolveEmailDetailEndpoint(id);
  const response = await http.get(target);

  return mapEmailDetail(resolveEmailDetailPayload(response.data));
}

export async function sendEmail(input: SendEmailInput): Promise<void> {
  await http.post(endpoints.sendEmail, input);
}
