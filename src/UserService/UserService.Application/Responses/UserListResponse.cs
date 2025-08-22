namespace UserService.Application.Responses
{
    public class UserListResponse
    {
        public IEnumerable<UserResponse> Users { get; }
        public int TotalCount { get; }

        public UserListResponse(IEnumerable<UserResponse> users, int totalCount)
        {
            Users = users;
            TotalCount = totalCount;
        }
    }
}
