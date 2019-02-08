using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookService.Models
{
    public class Book
    {
        public string BookId { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
        //public Author Auther { get; set; }
    }
}
