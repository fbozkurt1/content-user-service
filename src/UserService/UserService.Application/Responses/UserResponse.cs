using UserService.Domain.Entities;

namespace UserService.Application.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserResponse(int id, string username, string email, DateTime createdAt)
        {
            Id = id;
            Username = username;
            Email = email;
            CreatedAt = createdAt;
        }

        public UserResponse(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Email = user.Email;
            CreatedAt = user.CreatedAt;
        }
    }
}
