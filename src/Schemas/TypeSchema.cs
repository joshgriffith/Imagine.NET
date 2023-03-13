using System;
using System.Reflection;

namespace Imagine.Schemas {
    internal class TypeSchema {
        public string Schema { get; set; }
        public Type Type { get; set; }
        public List<MemberInfo> Members { get; set; } = new();

        public TypeSchema(Type type) {
            Type = type;
        }
    }
}