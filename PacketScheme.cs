using System.Collections.Concurrent;
using System.Reflection;

namespace NetUtils;

public enum FieldType {
    Simple,
    List,
    Array
}

public class PacketField {
    public PropertyInfo Info { get; set; } = null!;
    public FieldType Type { get; set; }
    public TypeCode Code { get; set; }
}

public class PacketScheme {
    public static ConcurrentDictionary<Type, PacketScheme> Schemes = new();

    public Type Type { get; }
    public List<PacketField> Fields { get; } = new();

    public PacketScheme(Type type) {
        Type = type;
        Build();
        Schemes[Type] = this;
    }

    public void Build() {
        foreach (var prop in Type.GetProperties()) {
            if (!prop.CanWrite) continue;

            var field = new PacketField {
                Info = prop,
                Code = Type.GetTypeCode(prop.PropertyType)
            };

            var type = prop.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
                field.Code = Type.GetTypeCode(type.GetGenericArguments()[0]);
                field.Type = FieldType.List;
            } else if (type.IsArray) {
                field.Code = Type.GetTypeCode(type.GetElementType());
                field.Type = FieldType.Array;
            }

            Fields.Add(field);
        }
    }
}