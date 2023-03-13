using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionToCodeLib;

namespace Imagine.Queryable {
    public class ImaginaryQueryProvider : IQueryProvider {
        
        private readonly Imagination _imagination;
        private readonly object _data;
        private readonly string _prompt;
        private readonly int _count;
        
        public ImaginaryQueryProvider(Imagination imagination, object data = null, string prompt = "", int count = 5) {
            _imagination = imagination;
            _data = data;
            _prompt = prompt;
            _count = count;
        }

        public IQueryable CreateQuery(Expression expression) {
            var type = expression.Type.GetElementType();

            try {
                return (IQueryable) Activator.CreateInstance(typeof(ImaginaryQuery<>).MakeGenericType(type), this, expression);
            }
            catch (TargetInvocationException exception) {
                throw exception.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
            return new ImaginaryQuery<TElement>(this, expression);
        }

        public object Execute(Expression expression) {
            throw new NotImplementedException("Todo: ImaginaryQueryProvider.Execute");
        }

        public TResult Execute<TResult>(Expression expression) {
            var type = typeof(TResult).GetGenericArguments().First();

            var method = GetType()
                .GetMethod(nameof(InternalExecute), BindingFlags.Instance | BindingFlags.NonPublic)?
                .MakeGenericMethod(type);

            var task = method.Invoke(this, new object[] { expression }) as Task<IList>;
            var results = task.ConfigureAwait(false).GetAwaiter().GetResult();

            return (TResult) results;
        }

        private async Task<IList> InternalExecute<T>(Expression expression) {
            var code = ExpressionToCode.ToCode(expression);

            code = code.Replace("\"", "'");
            
            if (code == "this") {
                return await _imagination.ImagineInternal<T>(_data, _prompt, _count);
            }
            
            return await _imagination.ImagineInternal<T>(code, _prompt, _count);
        }
    }
}