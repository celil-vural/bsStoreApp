﻿using Entities.Dto;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using Services.Contracts;

namespace Services.Concrete
{
    public class BookLinks : IBookLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<BookDto> _dataShaper;
        public BookLinks(LinkGenerator linkGenerator, IDataShaper<BookDto> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }

        public LinkResponse TryGenerateLinks(IEnumerable<BookDto> booksDto, string fields, HttpContext httpContext)
        {
            var shapedBooks = ShapeData(booksDto, fields);
            if (ShouldGenerateLinks(httpContext))
                return ReturnLinkedBooks(booksDto, fields, httpContext, shapedBooks);
            return ReturnShapedBooks(shapedBooks);
        }

        private LinkResponse ReturnLinkedBooks(IEnumerable<BookDto> booksDto, string fields,
            HttpContext httpContext, List<Entity> shapedBooks)
        {
            var bookDtoList = booksDto.ToList();
            for (int i = 0; i < bookDtoList.Count; i++)
            {
                var bookLinks = CreateForBook(httpContext, bookDtoList[i], fields);
                shapedBooks[i].Add("Links", bookLinks);
            }

            var bookCollection = new LinkCollectionWrapper<Entity>(shapedBooks);
            CreateForBooks(httpContext, bookCollection);
            return new LinkResponse()
            {
                HasLinks = true,
                LinkedEntities = bookCollection
            };
        }

        private LinkCollectionWrapper<Entity> CreateForBooks(HttpContext httpContext,
            LinkCollectionWrapper<Entity> bookCollectionWrapper)
        {
            bookCollectionWrapper.Links.Add(new Link()
            {
                Href = $"/api/{httpContext.GetRouteData().Values["controller"].ToString()?.ToLower()}",
                Rel = "self",
                Method = "GET"
            });
            return bookCollectionWrapper;
        }
        private List<Link> CreateForBook(HttpContext httpContext, BookDto dto, string fields)
        {
            var controllerName = httpContext.GetRouteData().Values["controller"].ToString()?.ToLower();
            var links = new List<Link>()
            {
                new Link()
                {
                    Href = $"/api/{controllerName}/{dto.Id}",
                    Rel = "self",
                    Method = "GET"
                },
                new Link()
                {
                    Href = $"/api/{controllerName}",
                    Rel = "create",
                    Method = "POST"
                },
                new Link()
                {
                    Href = $"/api/{controllerName}/{dto.Id}",
                    Rel = "update",
                    Method = "PUT"
                },
                new Link()
                {
                    Href = $"/api/{controllerName}/{dto.Id}",
                    Rel = "delete",
                    Method = "DELETE"
                },
            };
            return links;
        }

        private LinkResponse ReturnShapedBooks(List<Entity> shapedBooks)
        {
            return new LinkResponse()
            {
                ShapedEntities = shapedBooks
            };
        }

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];
            return mediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
        }

        private List<Entity> ShapeData(IEnumerable<BookDto> booksDto, string fields)
        {
            return _dataShaper.ShapeData(booksDto, fields).Select(b => b.Entity).ToList();
        }
    }
}