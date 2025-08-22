using ContentService.Application.Requests;
using ContentService.Application.Responses;
using ContentService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.API.Controllers
{
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class ContentsController : ControllerBase
    {
        private readonly IContentService _contentService;

        public ContentsController(IContentService contentService)
        {
            _contentService = contentService;
        }

        /// <summary>
        /// Retrieves a paginated list of contents.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [ProducesResponseType<ContentListResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public async Task<IActionResult> GetContents(int page = 1, int pageSize = 20)
        {
            var result = await _contentService.GetAllContents(page, pageSize);
            if(result.Contents == null || !result.Contents.Any())
                return NoContent();

            return Ok(result);
        }

        /// <summary>
        /// Gets a specific content by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType<ContentResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContent(int id)
        {
            var content = await _contentService.GetContent(id);
            if (content == null)
                return NotFound();
            return Ok(content);
        }

        /// <summary>
        /// Creates a new content. While creating content, also validate if the associated user exists by calling UserService.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType<ContentResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateContent([FromBody] SaveContentRequest request)
        {
            if (request == null)
                return BadRequest("Invalid content data.");

            var content = await _contentService.CreateContent(request);
            return CreatedAtAction(nameof(GetContent), new { id = content.Id }, content);
        }

        /// <summary>
        /// Updates existing content. While updating content, also validate if the associated user exists by calling UserService.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType<ContentResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContent(int id, [FromBody] SaveContentRequest request)
        {
            if (request == null || id <= 0)
                return BadRequest("Invalid content data or ID.");

            var updatedContent = await _contentService.UpdateContent(id, request);
            if (updatedContent == null)
                return NotFound();
            return Ok(updatedContent);
        }

        /// <summary>
        /// Deletes a content by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid content ID.");
            var deleted = await _contentService.DeleteContent(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

    }
}
