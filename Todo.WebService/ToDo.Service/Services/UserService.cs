using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Core.DTOs;
using ToDo.Core.Entities;
using ToDo.Core.Interfaces.Services;

namespace ToDo.Service.Services
{
    public class UserService: IUserService
    {
        //private readonly IRabbitMqService _rabbitMqService;

        //public UserService(IRabbitMqService rabbitMqService)
        //{
        //    _rabbitMqService = rabbitMqService;
        //}

        //public Task SendUserToQueueAsync(CreateUserRequest request)
        //{
        //    return _rabbitMqService.PublishUserAsync(request);
        //}


        private readonly IRabbitMqRpc _rpc;

        public UserService(IRabbitMqRpc rpc)
        {
            _rpc = rpc;
        }

        public async Task<OperationResponse> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
        {
            // כאן אין צורך לבנות DTO חדש – CreateUserRequest כבר מכיל את השדות הנכונים
            return await _rpc.RequestUserAsync<CreateUserRequest, OperationResponse>(
                payload: request,
                timeout: TimeSpan.FromSeconds(10),
                ct: ct);
        }

    }
}
