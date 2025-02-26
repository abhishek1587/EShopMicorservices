using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.CQRS
{

    //when return response is not needed
    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit> where TCommand : ICommand<Unit>
    {
    }
    //when return response is needed
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
         where TCommand : ICommand<TResponse>
        where TResponse : notnull
    {
    }
}
