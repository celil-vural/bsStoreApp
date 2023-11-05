using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Concrete.EfCore;
using Repositories.Concrete.Extensions;
using Repositories.Contracts;

namespace Repositories.Concrete
{
    public sealed class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        public BookRepository(RepositoryContext context) : base(context)
        {
        }

        public async Task<PagedList<Book>> GetAllBooksAsync(BookParameters bookParameters, bool trackChanges = false)
        {
            var books = await FindAll()
                .FilterBooks(bookParameters)
                .ToListAsync();
            return PagedList<Book>.ToPagedList(books, bookParameters.PageNumber, bookParameters.PageSize);
        }


        public async Task<Book?> GetOneBookByIdAsync(int id, bool trackChanges = false) =>
            await FindByCondition(b => b.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

        public void CreateOneBook(Book book) => Create(book);

        public void DeleteOneBook(Book book) => Delete(book);
        public void UpdateOneBook(Book book) => Update(book);
        public async Task<IEnumerable<Book>> GetAllBooksAsync(bool trackChanges = false)
        {
            var books = await FindAll(trackChanges)
                .OrderBy(b => b.Id).ToListAsync();
            return books;
        }
    }
}
