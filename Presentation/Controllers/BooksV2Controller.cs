using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers
{
    //[ApiVersion("2.0",Deprecated = true)]
    [ApiController]
    //[Route("api/{v:apiVersion}/books")]
    [Route("api/books")]
    public class BooksV2Controller : ControllerBase
    {
        private readonly IServiceManager serviceManager;

        public BooksV2Controller(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooksAsync()
        {
            var books = await serviceManager.BookService.GetAllBooksAsync(false);
            return Ok(books);
        }
    }
}
