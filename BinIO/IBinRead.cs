using System;

namespace BinIO {

    public interface IBinRead {
        ulong ReadBits(byte numbits);
        Byte ReadByte();
        SByte ReadSByte();
        Int16 ReadInt16();
        UInt16 ReadUInt16();
        Int32 ReadInt32();
        UInt32 ReadUInt32();
        long ReadLong();
        ulong ReadULong();
        float ReadFloat();
        double ReadDouble();

        byte[] ReadToEnd();

        void Close();
    }

}