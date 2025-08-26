using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Core.Entities;

namespace ToDo.Core.Interfaces.Repositories
{
    public interface IItemRepository
    {
        Task AddItemAsync(Item item);              // שמירת משימה חדשה
        Task<Item?> GetItemByIdAsync(int id);      // שליפה לפי Id (רשות)
        Task SoftDeleteItemAsync(int itemId); // ✅ מחיקה רכה

    }
}
