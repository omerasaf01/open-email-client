using FastEndpoints;

namespace OpenEmail.Application.Features.Emails.Queries.FetchInbox;

public record FetchInboxQuery() : ICommand<FetchInboxResult>;