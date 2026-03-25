using OpenEmail.Application.Common.Dtos;

namespace OpenEmail.Application.Features.Emails.Queries.GetEmailById;

public record GetEmailByIdResult(EmailMessageDto  EmailMessage);