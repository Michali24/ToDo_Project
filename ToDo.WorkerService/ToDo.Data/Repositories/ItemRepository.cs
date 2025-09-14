using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Core.Entities;
using ToDo.Core.Interfaces.Repositories;

namespace ToDo.Data.Repositories
{
    public class ItemRepository:IItemRepository
    {
        private readonly AppDbContext _context;

        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }

        //Create new item
        public async Task AddItemAsync(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        //Mark item to Complete
        public async Task MarkItemAsCompletedAsync(int itemId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item != null && !item.IsDeleted)
            {
                item.IsCompleted = true;
                await _context.SaveChangesAsync();
            }
        }

        //Soft Delete to item
        public async Task SoftDeleteItemAsync(int itemId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item != null && !item.IsDeleted)
            {
                item.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
