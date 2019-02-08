using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookService.Service
{
    public interface IBookService
    {
        Task CreateTheDatabaseAsync();
        void InitAsync();
    }
}
