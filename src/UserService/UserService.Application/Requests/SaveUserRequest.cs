namespace UserService.Application.Requests
{
    public class SaveUserRequest
    {
        public int? Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
