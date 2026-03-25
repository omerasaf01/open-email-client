using FastEndpoints;

namespace OpenEmail.Application.Features.Emails.Queries.GetEmailById;

public record GetEmailByIdQuery(string Id) : ICommand<GetEmailByIdResult>;