namespace OpenEmail.Application.Common.Dtos;

public record EmailAttachmentDto
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public byte[] Data { get; set; }  
}