using FastEndpoints;

namespace OpenEmail.Application.Features.Emails.Commands.SendEmail;

public record SendEmailCommand(string To, string Subject, string Body) : ICommand;