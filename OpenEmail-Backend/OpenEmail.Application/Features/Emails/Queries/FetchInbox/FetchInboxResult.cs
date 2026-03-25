using OpenEmail.Application.Common.Dtos;

namespace OpenEmail.Application.Features.Emails.Queries.FetchInbox;

public record FetchInboxResult(List<EmailSummaryDto> EmailSummaries);