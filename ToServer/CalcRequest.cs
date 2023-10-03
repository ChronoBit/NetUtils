using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetUtils.ToServer {
    [Packet(Opcode.CalculateExpression)]
    public class CalcRequest : PacketBody {
        public List<string> Operations { get; set; } = new();
    }
}
