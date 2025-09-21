using ToDo.Core.DTOs;
using ToDo.Core.Entities;
using ToDo.Core.Interfaces.Repositories;
using ToDo.Core.Interfaces.Services;

namespace ToDo.Service.Services
{
    public class UserService :IUserService
    {
        //private readonly IUserRepository _userRepository;

        //public UserService(IUserRepository userRepository)
        //{
        //    _userRepository = userRepository;
        //}

        //public async Task HandleNewUserAsync(CreateUserRequest request)
        //{
        //    var user = new User
        //    {
        //        Name = request.Name,
        //        Password=request.Password
        //    };

        //    await _userRepository.AddUserAsync(user);
        //}


        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<OperationResponse> HandleNewUserAsync(CreateUserRequest request)
        {
            try
            {
                var user = new User
                {
                    Name = request.Name,
                    Password = request.Password
                };

                await _userRepository.AddUserAsync(user);

                return new OperationResponse
                {
                    Id = user.Id,
                    Success = true,
                    Message = $"User '{user.Name}' created successfully ✅",
                    ExecutedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    Message = "Failed to create user ❌",
                    ExecutedAt = DateTime.UtcNow
                };
            }
        }
    }
}
