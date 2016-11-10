using System;

[Serializable]
public class Packet
{
    public bool syn;
    public bool ack;
    public bool fin;
    public int sequenceNumber;
    public int ackNumber;
    public int data;

    public Packet(bool syn, bool ack, bool fin, int sequenceNumber, int ackNumber, int data)
    {
        this.syn = syn;
        this.ack = ack;
        this.fin = fin;
        this.sequenceNumber = sequenceNumber;
        this.ackNumber = ackNumber;
        this.data = data;
    }
}
