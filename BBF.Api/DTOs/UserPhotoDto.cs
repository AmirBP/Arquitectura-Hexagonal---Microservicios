namespace BBF.Api.DTOs
{
    public class UserPhotoDto
    {
        public int UserId { get; set; }
        public string PhotoBase64 { get; set; } = string.Empty;
    }
}
