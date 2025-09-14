using ToDo.Core.DTOs;
using ToDo.Core.Entities;
using ToDo.Core.Interfaces.Repositories;
using ToDo.Core.Interfaces.Services;

namespace ToDo.Service.Services
{
    public class UserService :IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task HandleNewUserAsync(CreateUserRequest request)
        {
            var user = new User
            {
                Name = request.Name,
                Password=request.Password
            };

            await _userRepository.AddUserAsync(user);
        }
    }
}
