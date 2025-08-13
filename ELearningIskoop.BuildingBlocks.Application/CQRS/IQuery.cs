using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace ELearningIskoop.BuildingBlocks.Application.CQRS

{
    // CQRS Query pattern için temel interface
    // Veri okuma operasyonları için kullanılır
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {

    }

    // Query handler'lar için temel interface
    //TQuery andle edilecek query tipi
    //TResponse Handler'ın döneceği sonuç tipi
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
    }



}
