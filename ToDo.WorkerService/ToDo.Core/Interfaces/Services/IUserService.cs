using ToDo.Core.DTOs;

namespace ToDo.Core.Interfaces.Services
{
    public interface IUserService
    {
        //Task HandleNewUserAsync(CreateUserRequest request);

        Task<OperationResponse> HandleNewUserAsync(CreateUserRequest request);

    }
}
