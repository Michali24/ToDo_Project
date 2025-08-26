using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Core.Entities;

namespace ToDo.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);              // שמירת משתמש חדש
        Task<User?> GetUserByIdAsync(int id);      // שליפת משתמש לפי Id
        Task<bool> UserExistsAsync(int id);        // בדיקה אם קיים משתמש
    }
}
