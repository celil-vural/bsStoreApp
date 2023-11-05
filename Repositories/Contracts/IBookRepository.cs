using Entities.Models;
using Entities.RequestFeatures;

namespace Repositories.Contracts
{
    public interface IBookRepository : IRepositoryBase<Book>
    {
        Task<PagedList<Book>> GetAllBooksAsync(BookParameters bookParameters, bool trackChanges = false);
        Task<Book?> GetOneBookByIdAsync(int id, bool trackChanges = false);
        void CreateOneBook(Book book);
        void DeleteOneBook(Book book);
        void UpdateOneBook(Book book);
        Task<IEnumerable<Book>> GetAllBooksAsync(bool trackChanges = false);
    }
}
