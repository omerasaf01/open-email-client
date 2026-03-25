using FastEndpoints;
using OpenEmail.Api.Contracts.Requests;
using OpenEmail.Api.Contracts.Responses;
using OpenEmail.Application.Features.Emails.Queries.GetEmailById;

namespace OpenEmail.Api.Endpoints.Emails;

public class GetEmailByIdEndpoint : Endpoint<GetEmailByIdRequest, GetEmailByIdResponse>
{
    public override void Configure()
    {
        Get("/api/emails/{Id}");
        Summary(s => s.Summary = "Gets an email by Id");
    }

    public override async Task HandleAsync(GetEmailByIdRequest request, CancellationToken ct)
    {
        var emailMessage = await new GetEmailByIdQuery(request.Id)
            .ExecuteAsync(ct);

        await Send.OkAsync(new GetEmailByIdResponse(emailMessage.EmailMessage), cancellation: ct);
    }
}