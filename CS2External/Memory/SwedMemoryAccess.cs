using Swed64;
using System;
using System.Numerics;

namespace CS2External.Memory
{
    public class SwedMemoryAccess : IMemoryAccess
    {
        private readonly Swed _swed;

        public SwedMemoryAccess(string processName)
        {
            _swed = new Swed(processName);
        }

        public IntPtr GetModuleBase(string moduleName)
        {
            return _swed.GetModuleBase(moduleName);
        }

        public IntPtr ReadPointer(IntPtr address, int offset)
        {
            return _swed.ReadPointer(address, offset);
        }

        public IntPtr ReadPointer(IntPtr address)
        {
            return _swed.ReadPointer(address);
        }

        public uint ReadUInt(IntPtr address, int offset)
        {
            return _swed.ReadUInt(address + offset);
        }

        public void WriteUInt(IntPtr address, uint value)
        {
            _swed.WriteUInt(address, value);
        }
        
        public void WriteUInt(IntPtr address, int offset, uint value)
        {
            _swed.WriteUInt(address + offset, value);
        }

        public float ReadFloat(IntPtr address, int offset)
        {
            return _swed.ReadFloat(address + offset);
        }

        public void WriteFloat(IntPtr address, int offset, float value)
        {
            _swed.WriteFloat(address + offset, value);
        }

        public bool ReadBool(IntPtr address, int offset)
        {
            return _swed.ReadBool(address + offset);
        }

        public uint ReadUInt(nint v)
        {
            return _swed.ReadUInt((IntPtr)v);
        }

        public Vector2 ReadVec2(IntPtr address, int offset)
        {
            float x = _swed.ReadFloat(address + offset);
            float y = _swed.ReadFloat(address + offset + 4);
            return new Vector2(x, y);
        }

        public void WriteVec2(IntPtr address, int offset, Vector2 value)
        {
            _swed.WriteFloat(address + offset, value.X);
            _swed.WriteFloat(address + offset + 4, value.Y);
        }
        
        public Vector2 ReadVec2(IntPtr address)
        {
            float x = _swed.ReadFloat(address);
            float y = _swed.ReadFloat(address + 4);
            return new Vector2(x, y);
        }

        public void WriteVec2(IntPtr address, Vector2 value)
        {
            _swed.WriteFloat(address, value.X);
            _swed.WriteFloat(address + 4, value.Y);
        }
    }

    public interface IMemoryAccess
    {
        IntPtr GetModuleBase(string moduleName);
        IntPtr ReadPointer(IntPtr address, int offset);
        IntPtr ReadPointer(IntPtr address);
        
        uint ReadUInt(IntPtr address, int offset);
        uint ReadUInt(nint v); 

        void WriteUInt(IntPtr address, uint value);
        void WriteUInt(IntPtr address, int offset, uint value);
        
        float ReadFloat(IntPtr address, int offset);
        void WriteFloat(IntPtr address, int offset, float value);
        
        bool ReadBool(IntPtr address, int offset);

        Vector2 ReadVec2(IntPtr address, int offset);
        void WriteVec2(IntPtr address, int offset, Vector2 value);
        Vector2 ReadVec2(IntPtr address);
        void WriteVec2(IntPtr address, Vector2 value);
    }
}