using System;

namespace Spate
{
    /// <summary>
    /// 安全类型uint
    /// </summary>
    public struct _UInt
    {
        private static Random rnd;
        static _UInt()
        {
            rnd = new Random(Environment.TickCount);
        }

        private uint encValue;
        private byte xorKey;

        public _UInt(uint defValue = 0)
        {
            encValue = 0;
            xorKey = 0;
            Value = defValue;
        }

        public uint Value
        {
            get
            {
                return xorKey ^ encValue;
            }
            set
            {
                xorKey = (byte)rnd.Next(0, byte.MaxValue);
                encValue = xorKey ^ value;
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is _UInt) && ((_UInt)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        //public static implicit operator uint(_UInt s)
        //{
        //    return s.Value;
        //}

        //public static implicit operator _UInt(byte v)
        //{
        //    return new _UInt(v);
        //}

        //public static implicit operator _UInt(ushort v)
        //{
        //    return new _UInt(v);
        //}

        //public static implicit operator _UInt(uint v)
        //{
        //    return new _UInt(v);
        //}

        //public static bool operator ==(_UInt a, _UInt b)
        //{
        //    return a.Value == b.Value;
        //}

        //public static bool operator !=(_UInt a, _UInt b)
        //{
        //    return a.Value != b.Value;
        //}

        //public static _UInt operator ++(_UInt s)
        //{
        //    return new _UInt(s.Value + 1);
        //}

        //public static _UInt operator --(_UInt s)
        //{
        //    return new _UInt(s.Value - 1);
        //}
    }
}
