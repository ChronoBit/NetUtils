namespace NetUtils; 

public class PacketAttribute : Attribute {
    public PacketAttribute(Opcode opcode) {
        Opcode = opcode;
    }

    public Opcode Opcode { get; }
}