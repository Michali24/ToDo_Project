using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ToDo.Core.DTOs;
using ToDo.Core.Interfaces.Services;
using ToDo.Core.Settings;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        //private readonly IUserService _userService;

        //public UserController(IUserService userService)
        //{
        //    _userService = userService;
        //}

        //private readonly IUserService _userService;
        //private readonly IRabbitMqRpc _rpc; // ✅ שימוש ב-RPC

        //public UserController(IUserService userService, IRabbitMqRpc rpc)
        //{
        //    _userService = userService;
        //    _rpc = rpc;
        //}


        //private readonly IRabbitMqRpc _rpc;              // שימוש ב-RPC
        //private readonly ILogger<UserController> _logger;
        //private readonly RabbitMqSettings _settings;

        //public UserController(IRabbitMqRpc rpc,
        //                          ILogger<UserController> logger,
        //                          IOptions<RabbitMqSettings> options)
        //{
        //    _rpc = rpc;
        //    _logger = logger;
        //    _settings = options.Value;
        //}



        //[HttpPost("create")]
        //public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        //{
        //    await _userService.SendUserToQueueAsync(request);
        //    return Accepted("User creation sent to queue");
        //}

        //[HttpPost("create")]
        //public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken ct)
        //{
        //    try
        //    {
        //        // שליחת בקשה ל-Worker והמתנה לתשובה
        //        var response = await _rpc.RequestAsync<CreateUserRequest, ItemResponse>(
        //            _settings.UserQueue,
        //            request,
        //            TimeSpan.FromSeconds(10),
        //            ct);

        //        if (response.Success)
        //        {
        //            return Ok(new { message = "User created successfully ✅" });
        //        }

        //        return BadRequest(new { message = response.ErrorMessage ?? "Failed to create user ❌" });
        //    }
        //    catch (TimeoutException)
        //    {
        //        _logger.LogWarning("⏳ Timeout while waiting for CreateUser response for user {UserName}", request.Name);
        //        return StatusCode(504, new { message = "Timeout waiting for worker response ⏳" });
        //    }
        //}




        //private readonly IRabbitMqRpc _rpc;
        //public UserController(IRabbitMqRpc rpc) => _rpc = rpc;                                  // DI של השירות

        //[HttpPost]
        //public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest dto)
        //{
        //    // שולח ל-UserQueue, מחכה לתשובה מה-Worker, ואז מחזיר 200 עם התשובה
        //    var result = await _rpc.RequestUserAsync<CreateUserRequest, UserResponse>(dto, TimeSpan.FromSeconds(10));
        //    return Ok(result);
        //}


        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// יצירת משתמש חדש: שולח בקשת RPC ל-Worker ומחזיר תשובה רק כשהפעולה הסתיימה ב-DB.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest dto, CancellationToken ct)
        {
            try
            {
                var result = await _userService.CreateUserAsync(dto, ct);

                if (result.Success)
                    return CreatedAtAction(nameof(CreateUser), result);

                return BadRequest(new { message = result.ErrorMessage ?? "Failed to create user ❌" });
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("⏳ Timeout while waiting for CreateUser response for user {UserName}", dto.Name);
                return StatusCode(504, new { message = "Timeout waiting for worker response ⏳" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating user");
                return StatusCode(500, new { message = "Internal Server Error 🛑" });
            }
        }

    }
}

