﻿namespace UserManagement.Services.CommandHandlers.StateCommandHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data.Extensions;
    using Domain.Commands.StateCommands;
    using Domain.Dtos;
    using Domain.Interfaces.Data;
    using Domain.Model;
    using MediatR;

    public class StateCommandHandlers
    {
        public class RegisterStateCommandHandler : IRequestHandler<RegisterStateCommand, ResponseDto<State>>
        {
            private readonly IDatabaseContext _context;
            public RegisterStateCommandHandler(IDatabaseContext databaseContext)
            {
                _context = databaseContext;
            }

            public async Task<ResponseDto<State>> Handle(RegisterStateCommand request, CancellationToken cancellationToken)
            {
                var response = new ResponseDto<State>();
                var userId = await GetId(cancellationToken);

                List<SPParameter> parameters = new()
                {
                    new SPParameter { Name = "@StateId", Value = userId, Type = TypeCode.Int32 },
                    new SPParameter { Name = "@Name", Value = request.Name, Type = TypeCode.String },
                    new SPParameter { Name = "@Code", Value = request.Code, Type = TypeCode.String },
                    new SPParameter { Name = "@CountryId", Value = request.CountryId, Type = TypeCode.Int32 }
                };
                var res = await _context.States.ExecuteSPAsync("dbo.PostState", cancellationToken, parameters);
                if (res.Any())
                {
                    response.Message = "Added Succesfully";
                    response.Result = res.First();
                }
                else
                {
                    response.Message = "Error Inserting..";
                }

                return response;
            }

            private async Task<int> GetId(CancellationToken cancellationToken)
            {
                var res = await _context.States.ExecuteSPAsync("dbo.GetStates", cancellationToken);
                return res.Count == 0 ? 1 : res.Count + 1;
            }
        }
        public class DeleteStateByIdCommandHandler : IRequestHandler<DeleteStateByIdCommand, ResponseDto<bool>>
        {
            private readonly IDatabaseContext _context;
            public DeleteStateByIdCommandHandler(IDatabaseContext databaseContext)
            {
                _context = databaseContext;
            }

            public async Task<ResponseDto<bool>> Handle(DeleteStateByIdCommand request, CancellationToken cancellationToken)
            {
                var response = new ResponseDto<bool>();
                List<SPParameter> parameters = new()
                {
                    new SPParameter { Name = "@StateId", Value = request.StateId, Type = TypeCode.Int32 }
                };
                var res = await _context.Cities.ExecuteSPAsync("dbo.DeleteStateById", cancellationToken, parameters);
                if (!res.Any())
                {
                    response.Message = "Deleted Succesfully";
                    response.Result = true;
                }
                else
                {
                    response.Message = "Error Deleting..";
                    response.Result = false;
                }

                return response;
            }
        }
    }
}
