using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetUtils {
    public class NetControl : IDisposable {
        public NetworkStream Stream { get; }
        private readonly SemaphoreSlim _semaphore = new(1);

        public NetControl(NetworkStream stream) {
            Stream = stream;
        }

        public async Task ParseData(byte[] data) {
            await using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms, Encoding.UTF8, true);
            var op = (Opcode)br.ReadByte();
            if (!OpcodeControl.Instance.Packets.TryGetValue(op, out var type)) return;
            var obj = Activator.CreateInstance(type);
            type.GetField("Net")?.SetValue(obj, this);
            type.GetMethod("Parse")!.Invoke(obj, new object?[] { br });
            var task = (Task)type.GetMethod("Handle")!.Invoke(obj, null)!;
            await task;
        }

        public async Task SendData(byte[] data) {
            await using var ms = new MemoryStream();
            await using var bw = new BinaryWriter(ms);
            bw.Write((short)0x6529);
            bw.Write(data.Length);
            bw.Write(data);
            await _semaphore.WaitAsync();
            try {
                await Stream.WriteAsync(ms.ToArray());
            } finally {
                _semaphore.Release();
            }
        }

        public async Task Send(PacketBody packet) {
            await using var ms = new MemoryStream();
            await using var bw = new BinaryWriter(ms);
            bw.Write((byte)packet.Opcode);
            packet.Build(bw);
            await SendData(ms.ToArray());
        }

        public void Dispose() {
            _semaphore.Dispose();
        }
    }
}
