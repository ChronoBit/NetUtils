using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetUtils {
    public class OpcodeControl {
        public static OpcodeControl Instance { get; } = new();

        public Dictionary<Opcode, Type> Packets { get; } = new();
        public Dictionary<Type, Opcode> Opcodes { get; } = new();

        public OpcodeControl() {
            var space = GetType().Namespace;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                foreach (var type in assembly.GetTypes()) {
                    var list = type.GetCustomAttributes(typeof(PacketAttribute), true);
                    if (list.Length == 0) continue;
                    var attr = list[0] as PacketAttribute;
                    Opcodes[type] = attr!.Opcode;
                    if (type.FullName!.StartsWith(space!)) continue;
                    Packets[attr.Opcode] = type;
                }
            }
        }
    }
}
