using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace ELearningIskoop.BuildingBlocks.Application.Behaviors
{
    // MediatR pipeline'ında validation yapmak için behavior
    // Bu sınıf, gelen command veya query'lerin doğrulamasını yapar(TRequest  Validate edilecek request tipi , TResponse Response tipi)
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            // Validation işlemi için context oluşturulur
            var context = new ValidationContext<TRequest>(request);

            // Tüm validator'lar çalıştırılır
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            //hata varsa, hata mesajları toplanır
            var failures = validationResults
                .SelectMany(result => result.Errors)
                .Where(failure => failure != null)
                .ToList();

            // Eğer hata varsa, ValidationException fırlatılır
            if (failures.Any())
            {
                var errorMessage = string.Join(", ", failures.Select(f => f.ErrorMessage));
                throw new ValidationException($"Validation failed for {typeof(TRequest).Name}: {errorMessage}");
            }

            return await next();
        }
    }
}
