using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetUtils {
    public partial class PacketBody {
        public NetControl? Net;

        public Opcode Opcode => OpcodeControl.Instance.Opcodes[GetType()];

        public async Task Send() {
            if (Net == null) return;
            await Net.Send(this);
        }

        public virtual Task Handle() {
            // Packet logic
            return Task.CompletedTask;
        }
    }
}
