using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;

namespace Entities.Dto
{
    public record LinkParameters
    {
        public BookParameters BookParameters { get; init; }
        public HttpContext HttpContext { get; set; }
    }
}
