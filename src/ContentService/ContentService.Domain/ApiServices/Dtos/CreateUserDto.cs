namespace ContentService.Domain.ApiServices.Dtos
{
    public class CreateUserDto
    {
        public string Username { get; }
        public string Email { get; }

        public CreateUserDto(string username, string email)
        {
            Username = username;
            Email = email;
        }
    }
}
