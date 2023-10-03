using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetUtils.ToServer {
    [Packet(Opcode.AliveTick)]
    public class AliveTick : PacketBody {
    }
}
