using System.Text;

namespace Imagine.Schemas {
    internal static class JsonSchemaProvider {

        public static string GetSchema(Type type) {

            var builder = new StringBuilder();
            builder.Append("{ ");

            var index = 0;

            foreach (var property in type.GetProperties().Where(each => each.CanWrite).OrderByDescending(each => each.Name)) {

                var jsonType = ResolveType(property.PropertyType);

                if(jsonType == "array") {
                    continue;
                }

                if (index > 0) {
                    builder.Append(", ");
                }

                if (property.PropertyType.IsClass && jsonType == "object") {
                    builder.Append(property.Name + ": " + GetSchema(property.PropertyType));
                    continue;
                }

                builder.Append(property.Name + ": " + jsonType);
                index += 1;
            }

            builder.Append(" }");

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

            return "object";
        }

        private static Type GetEnumerableType(Type type) {
            return (from intType in type.GetInterfaces()
                    where intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    select intType.GetGenericArguments()[0]).FirstOrDefault();
        }
    }
}