using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.Specifications
{
    internal class NotSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _specification;

        public NotSpecification(Specification<T> specification)
        {
            _specification = specification;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var expression = _specification.ToExpression();
            var parameter = expression.Parameters[0];
            var body = Expression.Not(expression.Body);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }

}
