using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.Behaviors;
using ELearningIskoop.Courses.Application.Behaviors;
using ELearningIskoop.Courses.Application.Services;
using ELearningIskoop.Courses.Domain.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ELearningIskoop.Courses.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // Courses Application katmanını DI container'a ekler
        public static IServiceCollection AddCourseApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // MediatR için assembly'i ekle
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CourseValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

            });

            // FluentValidation için assembly'i ekle
            services.AddValidatorsFromAssembly(assembly);

            //app service
            services.AddScoped<ICourseApplicationService, CourseApplicationService>();

            //domain service
            services.AddScoped<CourseDomainService>();

            return services;
        }
    }
}
