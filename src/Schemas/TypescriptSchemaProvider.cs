using System.Reflection;
using System.Text;

namespace Imagine.Schemas {
    internal static class TypescriptSchemaProvider {

        public static TypeSchema GetSchema(Type type) {

            var schema = new TypeSchema(type);
            var types = new List<Type> { type };

            void TryAddType(Type type) {
                if (type.IsClass && !type.Namespace.StartsWith("System") && !types.Contains(type)) {
                    types.Add(type);
                }
            }
            
            foreach (var property in GetProperties(type)) {
                TryAddType(property.PropertyType);
                schema.Members.Add(property);
            }

            foreach (var field in GetFields(type)) {
                TryAddType(field.FieldType);
                schema.Members.Add(field);
            }

            schema.Schema = string.Join(Environment.NewLine, types.Select(InternalGetSchema));

            return schema;
        }

        private static string InternalGetSchema(Type type) {
            var builder = new StringBuilder();
            builder.AppendLine("class " + type.Name + " {");
            
            foreach (var property in GetProperties(type)) {
                builder.AppendLine($"  {property.Name}: {ResolveType(property.PropertyType)}");
            }

            foreach (var field in GetFields(type)) {
                builder.AppendLine($"  {field.Name}: {ResolveType(field.FieldType)}");
            }

            builder.AppendLine("}");

            return builder.ToString();
        }

        private static string ResolveType(Type type) {

            if(type == typeof(DateTime)) {
                return "date";
            }

            if (type == typeof(bool)) {
                return "boolean";
            }

            if(type.IsEnum) {
                return "string";
            }

            if (type == typeof(int) || type == typeof(decimal) || type == typeof(double) || type == typeof(float)) {
                return "number";
            }

            if (type == typeof(string)) {
                return "string";
            }

            if (type.IsGenericType && GetEnumerableType(type) != null) {
                return "array";
            }

            return type.Name;
        }

        private static List<PropertyInfo> GetProperties(Type type) {
            return type.GetProperties().Where(each => each.CanWrite).OrderByDescending(each => each.Name).ToList();
        }

        private static List<FieldInfo> GetFields(Type type) {
            return type.GetFields().OrderByDescending(each => each.Name).ToList();
        }

        private static Type GetEnumerableType(Type type) {
            return (from each in type.GetInterfaces()
                where each.IsGenericType && each.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                select each.GetGenericArguments()[0]).FirstOrDefault();
        }
    }
}