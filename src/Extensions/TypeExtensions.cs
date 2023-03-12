namespace Imagine.Extensions {
    public static class TypeExtensions {
        
        public static bool HasInterface<T>(this Type type) {
            return type.GetInterfaces().Any(each => each == typeof(T));
        }

        public static bool HasInterface(this Type type, Type interfaceType) {
            if (interfaceType.IsGenericType) {
                return type.GetInterfaces().Any(each => each.IsGenericType && each.GetGenericTypeDefinition() == interfaceType);
            }

            return interfaceType.IsAssignableFrom(type);
        }
    }
}