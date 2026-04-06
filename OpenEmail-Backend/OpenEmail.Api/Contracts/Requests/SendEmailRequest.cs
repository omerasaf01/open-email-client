namespace OpenEmail.Api.Contracts.Requests;

public record SendEmailRequest(string To, string Subject, string Body);