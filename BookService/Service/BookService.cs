using BookService.DbContext;
using BookService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookService.Service
{
    public class BookService : IBookService
    {
        private BookdbContext _booksContext;
        public BookService(BookdbContext booksContext) =>
            _booksContext = booksContext ?? throw new ArgumentNullException(nameof(BookdbContext));

        public async Task CreateTheDatabaseAsync()
        {
            
            var created = await _booksContext.Database.EnsureCreatedAsync();
            
        }

        public  void InitAsync()
        {
            Book bookModel = new Book
            {
                BookId = Guid.NewGuid().ToString()
                ,
                Title = "Professional C# 7 and .NET Core 2.0"
                ,
                Publisher = "Wrox Press"
               //,
               // Auther = new Author()
               // {
               //     AuthorId = "GSN100"
               // ,
               //     FirstName = "Guman"
               // ,
               //     LastName = "Nagar"
               // }
            };
            _booksContext.Books.Add(bookModel);
             int changed = _booksContext.SaveChanges();          
        }

        
    }
}

