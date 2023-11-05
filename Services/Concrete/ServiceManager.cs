using AutoMapper;
using Repositories.Contracts;
using Services.Contracts;

namespace Services.Concrete
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IBookService> bookService;
        public ServiceManager(IRepositoryManager repositoryManager, ILoggerService logger, IMapper mapper,
            IBookLinks bookLinks)
        {
            bookService = new Lazy<IBookService>(() =>
                new BookService(repositoryManager, logger, mapper, bookLinks));
        }
        public IBookService BookService => bookService.Value;
    }
}
