using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ToDo.Core.DTOs;
using ToDo.Core.Interfaces.Services;
using ToDo.Core.Settings;
using ToDo.Service.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        //private readonly IItemService _itemService;

        //public ItemController(IItemService itemService)
        //{
        //    _itemService = itemService;
        //}

        //private readonly IRabbitMqRpc _rpc; // ✅ שימוש ב-RPC
        //private readonly ILogger<ItemController> _logger;
        //private readonly RabbitMqSettings _settings;

        //public ItemController(IRabbitMqRpc rpc, Logger<ItemController> logger,
        //                      IOptions<RabbitMqSettings> options)
        //{
        //    _rpc = rpc;
        //    _logger = logger;
        //    _settings = options.Value;
        //}


        //[HttpPost("create")]
        //public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest request)
        //{
        //    await _itemService.SendItemToQueueAsync(request);
        //    return Accepted("Item sent to queue");
        //}

        //[HttpPost("create")]
        //public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest request)
        //{
        //    try
        //    {
        //        // שולחים בקשת RPC ומחכים לתשובת ה-Worker
        //        var result = await _rpc.RequestAsync<CreateItemRequest, ItemResponse>(
        //            requestQueue: "item_queue",
        //            payload: request,
        //            timeout: TimeSpan.FromSeconds(10) // אפשר להגדיל אם ה-Worker איטי
        //        );

        //        return CreatedAtAction(nameof(CreateItem), result);
        //    }
        //    catch (TimeoutException)
        //    {
        //        return StatusCode(504, "Worker did not respond in time");
        //    }
        //}


        //[HttpPatch("complete/{id:int}")]
        //public async Task<IActionResult> CompleteItem(int id)
        //{
        //    await _itemService.CompleteItemAsync(id);
        //    return NoContent();
        //}



        //[HttpDelete("{id:int}/delete")]
        //public async Task<IActionResult> DeleteItem(int id)
        //{
        //    await _itemService.SoftDeleteItemAsync(id);
        //    return NoContent();
        //}


        private readonly IItemService _itemService;
        private readonly ILogger<ItemController> _logger;

        public ItemController(IItemService itemService, ILogger<ItemController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        /// <summary>
        /// יצירת Item חדש: שולח בקשת RPC ל-Worker ומחזיר תשובה רק כשהפעולה ב-DB הסתיימה.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest request, CancellationToken ct)
        {
            try
            {
                var result = await _itemService.CreateItemAsync(request, ct);

                if (result.Success)
                    return CreatedAtAction(nameof(CreateItem), result);

                return BadRequest(new { message = result.ErrorMessage ?? "Failed to create item ❌" });
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("⏳ Timeout while waiting for CreateItem response");
                return StatusCode(504, new { message = "Timeout waiting for worker response ⏳" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating item");
                return StatusCode(500, new { message = "Internal Server Error 🛑" });
            }
        }

        /// <summary>
        /// סימון Item כהושלם (Complete)
        /// </summary>
        [HttpPatch("complete/{id:int}")]
        public async Task<IActionResult> CompleteItem(int id, CancellationToken ct)
        {
            try
            {
                var response = await _itemService.CompleteItemAsync(id, ct);

                if (response.Success)
                    return Ok(new { message = "Item completed successfully ✅", itemId = id });

                return BadRequest(new { message = response.ErrorMessage ?? "Failed to complete item ❌" });
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("⏳ Timeout while waiting for CompleteItem response for item {ItemId}", id);
                return StatusCode(504, new { message = "Timeout waiting for worker response ⏳" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while completing item {ItemId}", id);
                return StatusCode(500, new { message = "Internal Server Error 🛑" });
            }
        }

        /// <summary>
        /// מחיקה לוגית (Soft Delete) של Item
        /// </summary>
        [HttpDelete("{id:int}/delete")]
        public async Task<IActionResult> DeleteItem(int id, CancellationToken ct)
        {
            try
            {
                var response = await _itemService.SoftDeleteItemAsync(id, ct);

                if (response.Success)
                    return Ok(new { message = "Item soft-deleted successfully 🗑", itemId = id });

                return BadRequest(new { message = response.ErrorMessage ?? "Failed to delete item ❌" });
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("⏳ Timeout while waiting for DeleteItem response for item {ItemId}", id);
                return StatusCode(504, new { message = "Timeout waiting for worker response ⏳" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting item {ItemId}", id);
                return StatusCode(500, new { message = "Internal Server Error 🛑" });
            }
        }

    }
}
