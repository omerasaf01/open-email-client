namespace OpenEmail.Application.Common.Dtos;

public record EmailMessageDto
{
    public string Id { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string From { get; set; }
    public string BodyHtml { get; set; } = string.Empty;
    public string[] To { get; set; }
    public DateTime ReceivedAt { get; set; }
    public bool IsRead { get; set; }
    public bool HasAttachments { get; set; }
    public List<EmailAttachmentDto> Attachments { get; set; }
    public string FolderOrLabel { get; set; } = "Inbox";
}