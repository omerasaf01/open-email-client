using FastEndpoints;
using OpenEmail.Application.Features.Emails.Queries.FetchInbox;

namespace OpenEmail.Api.Endpoints.Inbox;

public class FetchInboxEndpoint : EndpointWithoutRequest<FetchInboxResult>
{
    public override void Configure()
    {
        Get("/api/inbox");
        Summary(s => s.Summary = "Fetch the user's inbox");
        Tags("Inbox");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var emailSummaries = await new FetchInboxQuery()
            .ExecuteAsync(ct);
        
        await Send.OkAsync(new FetchInboxResult(emailSummaries.EmailSummaries), cancellation: ct);
    }
}