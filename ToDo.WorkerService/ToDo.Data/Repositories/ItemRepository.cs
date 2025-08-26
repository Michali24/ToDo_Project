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

        public async Task AddItemAsync(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task MarkItemAsCompletedAsync(int itemId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item != null && !item.IsDeleted)
            {
                item.IsCompleted = true;
                await _context.SaveChangesAsync();
            }
        }

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
