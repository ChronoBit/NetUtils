using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetUtils.ToClient {
    public enum CalcError {
        Ok,
        InvalidInput,
        DivByZero
    }

    [Packet(Opcode.CalculationResult)]
    public class CalcResponse : PacketBody {
        public CalcError Error { get; set; }
        public double Result { get; set; }
    }
}
