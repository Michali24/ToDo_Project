using Microsoft.AspNetCore.Mvc;
using ToDo.Core.DTOs;
using ToDo.Core.Interfaces.Services;
using ToDo.Service.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest request)
        {
            await _itemService.SendItemToQueueAsync(request);
            return Accepted("Item sent to queue");
        }

        [HttpPatch("complete/{id:int}")]
        public async Task<IActionResult> CompleteItem(int id)
        {
            await _itemService.CompleteItemAsync(id);
            return NoContent();
        }

        [HttpDelete("{id:int}/delete")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _itemService.SoftDeleteItemAsync(id);
            return NoContent();
        }

    }
}
