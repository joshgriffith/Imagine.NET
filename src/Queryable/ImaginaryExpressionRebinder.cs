using System;
using System.Linq.Expressions;
using Imagine.Extensions;

namespace Imagine.Queryable {
    public class ImaginaryExpressionRebinder<T> : ExpressionVisitor {
        private readonly IQueryable<T> _target;

        public ImaginaryExpressionRebinder(IQueryable<T> target) {
            _target = target;
        }

        protected override Expression VisitConstant(ConstantExpression node) {

            if (node.Type.HasInterface(typeof(IsImaginaryQuery))) {
                return base.VisitConstant(Expression.Constant(_target.Provider));
            }

            return base.VisitConstant(node);
        }
    }
}