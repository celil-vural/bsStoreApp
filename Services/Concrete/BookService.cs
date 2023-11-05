using AutoMapper;
using Entities.Dto;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;

namespace Services.Concrete
{
    public class BookService : IBookService
    {
        private readonly IRepositoryManager manager;
        private readonly ILoggerService logger;
        private readonly IMapper mapper;
        private readonly IBookLinks bookLinks;
        public BookService(IRepositoryManager manager, ILoggerService logger, IMapper mapper, IBookLinks bookLinks)
        {
            this.manager = manager;
            this.logger = logger;
            this.mapper = mapper;
            this.bookLinks = bookLinks;
        }
        public async Task<(LinkResponse linkResponse, MetaData metaData)> GetAllBooksAsync(
            LinkParameters linkParameters, bool trackChanges = false)
        {
            if (!linkParameters.BookParameters.ValidPriceRange)
            {
                throw new PriceOutOfRangeBadRequestException();
            }
            var booksWithMetaData = await manager.Book.GetAllBooksAsync(linkParameters.BookParameters, trackChanges);
            var booksDto = mapper.Map<IEnumerable<BookDto>>(booksWithMetaData);
            var links = bookLinks.TryGenerateLinks(
                booksDto,
                linkParameters.BookParameters.Fields,
                linkParameters.HttpContext);
            return (linkResponse: links, metaData: booksWithMetaData.MetaData);
        }

        public async Task<BookDto> GetOneBookByIdAsync(int id, bool trackChanges = false)
        {
            var entity = await GetOneBookByIdAndCheckExists(id);
            return mapper.Map<BookDto>(entity);
        }

        public async Task<BookDto> CreateOneBookAsync(BookDtoForInsertion bookDto)
        {
            var entity = mapper.Map<Book>(bookDto);
            manager.Book.CreateOneBook(entity);
            await manager.SaveAsync();
            return mapper.Map<BookDto>(entity);
        }

        public async Task UpdateOneBookAsync(int id, BookDtoForUpdate dto, bool trackChanges = false)
        {
            var entity = await GetOneBookByIdAndCheckExists(id);
            entity = mapper.Map<Book>(dto);
            manager.Book.UpdateOneBook(entity);
            await manager.SaveAsync();
        }

        public async Task DeleteOneBookAsync(int id)
        {
            var entity = await GetOneBookByIdAndCheckExists(id);
            manager.Book.DeleteOneBook(entity);
            await manager.SaveAsync();
        }

        public async Task<(BookDtoForUpdate bookDtoForUpdate, Book book)> GetOneBookForPatchAsync(int id, bool trackChanges = false)
        {
            var book = await GetOneBookByIdAndCheckExists(id); ;
            var bookDtoForUpdate = mapper.Map<BookDtoForUpdate>(book);
            return (bookDtoForUpdate, book);
        }

        public async Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book)
        {
            mapper.Map(bookDtoForUpdate, book);
            await manager.SaveAsync();
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync(bool trackChanges = false)
        {
            var books = await manager.Book.GetAllBooksAsync(trackChanges);
            var dtos = mapper.Map<IEnumerable<BookDto>>(books);
            return dtos;
        }

        private async Task<Book> GetOneBookByIdAndCheckExists(int id, bool trackChanges = false)
        {
            var entity = await manager.Book.GetOneBookByIdAsync(id);
            if (entity == null)
            {
                logger.LogInfo($"The book with id:{id} could not found.");
                throw new BookNotFoundException(id);
            }

            return entity;
        }
    }
}
