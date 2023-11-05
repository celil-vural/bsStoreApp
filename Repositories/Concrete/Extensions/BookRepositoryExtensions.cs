using Entities.Models;
using Entities.RequestFeatures;
using System.Linq.Dynamic.Core;
namespace Repositories.Concrete.Extensions
{
    public static class BookRepositoryExtensions
    {
        public static IQueryable<Book> FilterBooks(this IQueryable<Book> books,
            BookParameters booksParameters)
        {
            return books
                .FilterBooksByPrice(booksParameters.MinPrice, booksParameters.MaxPrice)
                .Search(booksParameters.SearchTerm)
                .Sort(booksParameters.OrderBy);
        }
        public static IQueryable<Book> FilterBooksByPrice(this IQueryable<Book> books,
            uint minPrice = 0, uint maxPrice = uint.MaxValue)
        {
            return books.Where(book => (
               (book.Price >= minPrice) &&
               (book.Price <= maxPrice)
           ));
        }
        public static IQueryable<Book> Search(this IQueryable<Book> books, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return books;
            }
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return books.Where(b => b.Title.ToLower().Contains(lowerCaseTerm));
        }

        public static IQueryable<Book> Sort(this IQueryable<Book> books, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString)) return books.OrderBy(b => b.Id);
            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Book>(orderByQueryString);
            if (orderQuery is null || string.IsNullOrWhiteSpace(orderQuery)) return books.OrderBy(b => b.Id);
            return books.OrderBy(orderQuery);
        }
    }
}
