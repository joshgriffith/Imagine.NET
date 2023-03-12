using System.Collections;
using System.Linq.Expressions;

namespace Imagine.Queryable {
    public class ImaginaryQuery<T> : IOrderedQueryable<T>, IsImaginaryQuery {
        public Type ElementType => typeof(T);
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }

        public ImaginaryQuery(Imagination imagination, object data, string prompt = "", int count = 5) {
            Provider = new ImaginaryQueryProvider(imagination, data, prompt, count);
            Expression = Expression.Constant(this);
        }

        internal ImaginaryQuery(IQueryProvider provider, Expression expression) {
            Provider = provider;
            Expression = expression;
        }

        public IEnumerator<T> GetEnumerator() {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}