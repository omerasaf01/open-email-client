using OpenEmail.Application.Common.Dtos;

namespace OpenEmail.Api.Contracts.Responses;

public record FetchInboxResponse(List<EmailSummaryDto> EmailSummaries);