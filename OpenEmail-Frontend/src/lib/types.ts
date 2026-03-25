export type LoginInput = {
  email: string;
  password: string;
};

export interface ApiEnvelope<T> {
  data?: T;
}

export interface SignInRequestDto {
  email: string;
  password: string;
}

export interface SignInResponseDto {
  accessToken?: string;
  token?: string;
  jwt?: string;
  user?: UserSummary;
  profile?: UserSummary;
}

export interface InboxEmailSummaryDto {
  id?: string;
  emailId?: string;
  from?: string;
  sender?: string;
  fromAddress?: string;
  subject?: string;
  snippet?: string;
  preview?: string;
  folderOrLabel?: string;
  sentAt?: string;
  receivedAt?: string;
  createdAt?: string;
  date?: string;
  isRead?: boolean;
  read?: boolean;
  hasAttachments?: boolean;
}

export interface InboxResponseDto {
  emailSummaries?: InboxEmailSummaryDto[];
  items?: InboxEmailSummaryDto[];
  results?: InboxEmailSummaryDto[];
  data?: InboxEmailSummaryDto[];
}

export interface EmailDetailDto {
  id?: string;
  emailId?: string;
  from?: string;
  sender?: string;
  fromAddress?: string;
  to?: string | string[];
  recipient?: string | string[];
  toAddress?: string | string[];
  subject?: string;
  body?: string;
  content?: string;
  bodyHtml?: string;
  sentAt?: string;
  receivedAt?: string;
  createdAt?: string;
  date?: string;
  isRead?: boolean;
  read?: boolean;
}

export interface EmailDetailResponseDto {
  message?: EmailDetailDto;
  emailMessage?: EmailDetailDto;
  item?: EmailDetailDto;
  email?: EmailDetailDto;
}

export interface SendEmailRequestDto {
  to: string;
  subject: string;
  body: string;
}

export type UserSummary = {
  id?: string;
  fullName?: string;
  email?: string;
};

export type LoginResponse = {
  accessToken: string;
  user?: UserSummary;
};

export type EmailSummary = {
  id: string;
  from: string;
  subject: string;
  snippet: string;
  sentAt: string;
  isRead: boolean;
};

export type EmailDetail = {
  id: string;
  from: string;
  to: string;
  subject: string;
  body: string;
  sentAt: string;
  isRead: boolean;
};

export type SendEmailInput = {
  to: string;
  subject: string;
  body: string;
};
