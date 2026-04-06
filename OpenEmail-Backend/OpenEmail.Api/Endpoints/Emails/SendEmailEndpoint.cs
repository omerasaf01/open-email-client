using FastEndpoints;
using OpenEmail.Api.Contracts.Requests;
using OpenEmail.Application.Features.Emails.Commands.SendEmail;

namespace OpenEmail.Api.Endpoints.Emails;

public class SendEmailEndpoint : Endpoint<SendEmailRequest>
{
    public override void Configure()
    {
        Post("/api/emails");
        Summary(s => s.Summary = "Sends an email");
    }

    public override async Task HandleAsync(SendEmailRequest request, CancellationToken ct)
    {
        await new SendEmailCommand(request.To, request.Subject, request.Body)
            .ExecuteAsync(ct);

        await Send.OkAsync(ct);
    }
}