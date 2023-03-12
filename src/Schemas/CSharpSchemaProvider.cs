using System.Reflection;
using System.Text;

namespace Imagine.Schemas {
    internal static class CSharpSchemaProvider {

        public static string GetSchema(Type type) {

            var types = new List<Type> { type };
            
            foreach (var property in GetProperties(type)) {
                if (property.PropertyType.IsClass && !property.PropertyType.Namespace.StartsWith("System") && !types.Contains(property.PropertyType)) {
                    types.Add(property.PropertyType);
                }
            }

            return string.Join(Environment.NewLine, types.Select(InternalGetSchema));
        }

        private static string InternalGetSchema(Type type) {
            var builder = new StringBuilder();
            builder.AppendLine("public class " + type.Name + " {");
            
            foreach (var property in GetProperties(type)) {
                builder.AppendLine($"  public {property.PropertyType.Name} {property.Name};");
            }

            builder.AppendLine("}");

            return builder.ToString();
        }

        private static List<PropertyInfo> GetProperties(Type type) {
            return type
                .GetProperties()
                .Where(each => each.CanWrite)
                .OrderByDescending(each => each.Name)
                .ToList();
        }

        private static Type GetEnumerableType(Type type) {
            return (from intType in type.GetInterfaces()
                    where intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    select intType.GetGenericArguments()[0]).FirstOrDefault();
        }
    }
}