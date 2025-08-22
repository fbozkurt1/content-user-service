using Microsoft.AspNetCore.Mvc;
using UserService.Application.Requests;
using UserService.Application.Responses;
using UserService.Application.Services;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a paginated list of users.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        [ProducesResponseType<UserListResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public async Task<IActionResult> GetUsers(int page = 1, int pageSize = 20)
        {
            var result = await _userService.GetAllUsers(page, pageSize);
            if (result.Users == null || !result.Users.Any())
                return NoContent();
            return Ok(result);
        }
        /// <summary>
        /// Gets a specific user by their ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUser(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] SaveUserRequest request)
        {
            if (request == null)
                return BadRequest("Invalid user data.");

            var user = await _userService.CreateUser(request);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        /// <summary>
        /// Updates user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] SaveUserRequest request)
        {
            if (request == null || id <= 0)
                return BadRequest("Invalid user data or ID.");
            var updatedUser = await _userService.UpdateUser(id, request);
            if (updatedUser == null)
                return NotFound();
            return Ok(updatedUser);
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var isDeleted = await _userService.DeleteUser(id);
            if (!isDeleted)
                return NotFound();
            return NoContent();
        }
    }
}
