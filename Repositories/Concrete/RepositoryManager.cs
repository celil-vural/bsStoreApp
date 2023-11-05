using Repositories.Concrete.EfCore;
using Repositories.Contracts;

namespace Repositories.Concrete
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext context;
        private readonly Lazy<IBookRepository> _bookRepository;

        public RepositoryManager(RepositoryContext context)
        {
            this.context = context;
            _bookRepository = new Lazy<IBookRepository>(() => new BookRepository(this.context));
        }
        public IBookRepository Book => _bookRepository.Value;
        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
