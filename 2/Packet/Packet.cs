using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Packet
{
    public class Method
    {
        public static byte[] PacketToByteArray(PacketFormate packet)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, packet);
            return ms.ToArray();
        }

        public static byte[] PacketListToByteArray(List<PacketFormate> packets)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, packets);
            return ms.ToArray();
        }

        public static PacketFormate ByteArrayToPacket(byte[] bytes, int len)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            ms.Write(bytes, 0, len);
            ms.Seek(0, SeekOrigin.Begin);                       // Use to make the position from current to begin
            PacketFormate packet = (PacketFormate)bf.Deserialize(ms);
            return packet;
        }                

        public static List<PacketFormate> ByteArrayToPacketList(byte[] bytes, int len)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            ms.Write(bytes, 0, len);
            ms.Seek(0, SeekOrigin.Begin);           // Use to make the position from current to begin
            List<PacketFormate> packets = (List<PacketFormate>)bf.Deserialize(ms);  
            return packets;
        }
    }

    [Serializable]
    public struct PacketFormate
    {
        public bool syn;
        public bool ack;
        public bool fin;
        public int sequenceNumber;
        public int ackNumber;
        public int data;

        public PacketFormate(bool syn, bool ack, bool fin, int sequenceNumber, int ackNumber, int data)
        {
            this.syn = syn;
            this.ack = ack;
            this.fin = fin;
            this.sequenceNumber = sequenceNumber;
            this.ackNumber = ackNumber;
            this.data = data;
        }

        public PacketFormate(int sequenceNumber, int ackNumber, int data)
        {
            syn = false;
            ack = false;
            fin = false;
            this.sequenceNumber = sequenceNumber;
            this.ackNumber = ackNumber;
            this.data = data;
        }
    }
}   