using System.Collections;
using System.Reflection;
using System.Text;

namespace NetUtils {
    public partial class PacketBody {
        public byte[] Build() {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            Build(bw);
            return ms.ToArray();
        }

        public void Build(BinaryWriter bw) {
            var type = GetType();
            if (PacketScheme.Schemes.TryGetValue(type, out var exScheme)) {
                Build(exScheme, bw);
            } else {
                var scheme = new PacketScheme(GetType());
                Build(scheme, bw);
            }
        }

        protected void Write(BinaryWriter bw, TypeCode code, object value) {
            switch (code) {
                case TypeCode.Boolean:
                    bw.Write((bool)value);
                    break;
                case TypeCode.Byte:
                    bw.Write((byte)value);
                    break;
                case TypeCode.Char:
                    bw.Write((char)value);
                    break;
                case TypeCode.Int16:
                    bw.Write((short)value);
                    break;
                case TypeCode.Int32:
                    bw.Write((int)value);
                    break;
                case TypeCode.Int64:
                    bw.Write((long)value);
                    break;
                case TypeCode.UInt16:
                    bw.Write((ushort)value);
                    break;
                case TypeCode.UInt32:
                    bw.Write((uint)value);
                    break;
                case TypeCode.UInt64:
                    bw.Write((ulong)value);
                    break;
                case TypeCode.Double:
                    bw.Write((double)value);
                    break;
                case TypeCode.Single:
                    bw.Write((float)value);
                    break;
                case TypeCode.Decimal:
                    bw.Write((decimal)value);
                    break;
                case TypeCode.String:
                    var str = (string)value;
                    bw.Write(str.Length);
                    bw.Write(Encoding.UTF8.GetBytes(str));
                    break;
                case TypeCode.DateTime:
                    var dt = (DateTime)value;
                    bw.Write(dt.ToFileTimeUtc());
                    break;
            }
        }

        protected void WriteCollection(BinaryWriter bw, TypeCode itemType, object items) {
            switch (items) {
                case Array array: {
                    bw.Write(array.Length);
                    foreach (var item in array) {
                        Write(bw, itemType, item);
                    }
                    break;
                }
                case ICollection list: {
                    bw.Write(list.Count);
                    foreach (var item in list) {
                        Write(bw, itemType, item);
                    }
                    break;
                }
            }
        }

        protected void Build(PacketScheme scheme, BinaryWriter bw) {
            foreach (var p in scheme.Fields) {
                var val = p.Info.GetValue(this);
                switch (p.Type) {
                    case FieldType.Simple:
                        Write(bw, p.Code, val ?? throw new InvalidOperationException());
                        break;
                    case FieldType.Array:
                    case FieldType.List:
                        WriteCollection(bw, p.Code, val ?? throw new InvalidOperationException());
                        break;
                }
            }
        }
    }
}