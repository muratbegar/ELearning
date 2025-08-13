using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ELearningIskoop.BuildingBlocks.Application.CQRS
{

    // Represents a CQRS Command pattern interface that returns void.
    // Implement this interface for command requests in the application.
    public interface ICommand : IRequest
    {

    }

    // CQRS Command pattern için temel interface (TResponse dönen)
    // Veri değiştiren ve sonuç dönen operasyonlar için kullanılır

    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }

    // Command handler'lar için temel interface (void dönen)
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    {
    }

    // Command handler'lar için temel interface (TResponse dönen)
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
    }
}
