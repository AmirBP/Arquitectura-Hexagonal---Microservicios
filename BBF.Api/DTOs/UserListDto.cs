namespace BBF.Api.DTOs
{
    public class UserListDto
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public List<UserProfileDto> Users { get; set; } = new List<UserProfileDto>();
    }
}
