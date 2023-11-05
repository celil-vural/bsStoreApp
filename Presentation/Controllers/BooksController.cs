using Entities.Dto;
using Entities.Exceptions;
using Entities.RequestFeatures;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System.Text.Json;

namespace Presentation.Controllers
{
    //[Route("api/[controller]")]
    //[ServiceFilter(typeof(LogFilterAttribute))]
    //[ApiVersion("1.0")]
    //[Route("api/{v:apiVersion}/books")]
    [Route("api/books")]
    [ApiController]
    //[ResponseCache(CacheProfileName = "5mins")] bunu kullan
    [HttpCacheExpiration(MaxAge = 300)]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager manager;
        public BooksController(IServiceManager manager)
        {
            this.manager = manager;
        }
        [HttpHead]
        [HttpGet(Name = "GetAllBooksAsync")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        //[ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetAllBooksAsync([FromQuery] BookParameters bookParameters)
        {
            if (!bookParameters.ValidPriceRange)
            {
                throw new PriceOutOfRangeBadRequestException();
            }
            var linkParameters = new LinkParameters()
            {
                BookParameters = bookParameters,
                HttpContext = HttpContext
            };

            var result = await manager.BookService.GetAllBooksAsync(linkParameters);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));
            return result.linkResponse.HasLinks
                ? Ok(result.linkResponse.LinkedEntities)
                : Ok(result.linkResponse.ShapedEntities);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] int id)
        {
            var book = await manager.BookService.GetOneBookByIdAsync(id);
            return Ok(book);
        }

        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost(Name = "CreateOneBookAsync")]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {
            var book = await manager.BookService.CreateOneBookAsync(bookDto);
            return StatusCode(201, book);
        }
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBookAsync([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate bookDto)
        {
            if (id != bookDto.Id) return BadRequest();
            await manager.BookService.UpdateOneBookAsync(id, bookDto);
            return Ok(bookDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
        {
            if (id == null || id < 1) return BadRequest();
            await manager.BookService.DeleteOneBookAsync(id);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] int id,
            [FromBody] JsonPatchDocument<BookDtoForUpdate>? bookPatch)
        {
            if (id < 1 || bookPatch is null) return BadRequest();
            var result = await manager.BookService.GetOneBookForPatchAsync(id, false);
            bookPatch.ApplyTo(result.bookDtoForUpdate, ModelState);
            TryValidateModel(result.bookDtoForUpdate);
            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
            await manager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);
            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetBooksOptions()
        {
            Response.Headers.Add("Allow", "GET, PUT, POST, PATCH, DELETE, HEAD, OPTIONS");
            return NoContent();
        }
    }
}
