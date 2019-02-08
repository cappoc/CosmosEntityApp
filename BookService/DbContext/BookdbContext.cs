using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookService.Models;
using Microsoft.EntityFrameworkCore;
namespace BookService.DbContext
{
    public class BookdbContext: Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Book> Books { get; set; }
        public BookdbContext(DbContextOptions<BookdbContext> options)
        : base(options) {
           
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);

        //    optionsBuilder.UseCosmos("https://localhost:8081"
        //        , "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
        //        "BookDb");


        //}
    }
}
