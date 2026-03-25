export type LoginInput = {
  email: string;
  password: string;
};

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
