using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NetUtils {
    public partial class PacketBody {
        public void Parse(BinaryReader br) {
            var type = GetType();
            if (PacketScheme.Schemes.TryGetValue(type, out var exScheme)) {
                Parse(exScheme, br);
            } else {
                var scheme = new PacketScheme(GetType());
                Parse(scheme, br);
            }
        }

        protected object ReadObject(BinaryReader br, TypeCode code) {
            switch (code) {
                case TypeCode.Boolean:
                    return br.ReadBoolean();
                case TypeCode.Byte:
                    return br.ReadByte();
                case TypeCode.Char:
                    return br.ReadChar();
                case TypeCode.Int16:
                    return br.ReadInt16();
                case TypeCode.Int32:
                    return br.ReadInt32();
                case TypeCode.Int64:
                    return br.ReadInt64();
                case TypeCode.UInt16:
                    return br.ReadUInt16();
                case TypeCode.UInt32:
                    return br.ReadUInt32();
                case TypeCode.UInt64:
                    return br.ReadUInt64();
                case TypeCode.Double:
                    return br.ReadDouble();
                case TypeCode.Single:
                    return br.ReadSingle();
                case TypeCode.Decimal:
                    return br.ReadDecimal();
                case TypeCode.String:
                    var length = br.ReadInt32();
                    var str = Encoding.UTF8.GetString(br.ReadBytes(length));
                    return str;
                case TypeCode.DateTime:
                    return DateTime.FromFileTimeUtc(br.ReadInt64());
            }
            throw new InvalidOperationException("Invalid parsing");
        }

        protected void ReadCollection(BinaryReader br, TypeCode itemCode, FieldType type, PropertyInfo prop) {
            var count = br.ReadInt32();
            switch (type) {
                case FieldType.Array:
                    var obj = Array.CreateInstance(Type.GetType($"System.{itemCode}")!, count);
                    for (int i = 0; i < count; i++) {
                        obj.SetValue(ReadObject(br, itemCode), i);
                    }
                    prop.SetValue(this, obj);
                    break;
                case FieldType.List:
                    var list = prop.GetValue(this);
                    prop.PropertyType.GetMethod("Clear")!.Invoke(list, Array.Empty<object>());
                    for (int i = 0; i < count; i++) {
                        prop.PropertyType.GetMethod("Add")!.Invoke(list, new[] { ReadObject(br, itemCode) });
                    }
                    break;
            }
        }

        protected void Parse(PacketScheme scheme, BinaryReader br) {
            foreach (var p in scheme.Fields) {
                switch (p.Type) {
                    case FieldType.Simple:
                        p.Info.SetValue(this, ReadObject(br, p.Code));
                        break;
                    case FieldType.Array:
                    case FieldType.List:
                        ReadCollection(br, p.Code, p.Type, p.Info);
                        break;
                }
            }
        }
    }
}
