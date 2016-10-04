using System;

namespace Spate
{
    /// <summary>
    /// 安全类型ulong
    /// </summary>
    public struct _ULong
    {
        private static Random rnd;
        static _ULong()
        {
            rnd = new Random(Environment.TickCount);
        }

        private ulong encValue;
        private byte xorKey;

        public _ULong(ulong defValue = 0)
        {
            encValue = 0;
            xorKey = 0;
            Value = defValue;
        }

        public ulong Value
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
            return (obj is _ULong) && ((_ULong)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        //public static implicit operator ulong(_ULong s)
        //{
        //    return s.Value;
        //}

        //public static implicit operator _ULong(byte v)
        //{
        //    return new _ULong(v);
        //}

        //public static implicit operator _ULong(ushort v)
        //{
        //    return new _ULong(v);
        //}

        //public static implicit operator _ULong(uint v)
        //{
        //    return new _ULong(v);
        //}

        //public static implicit operator _ULong(ulong v)
        //{
        //    return new _ULong(v);
        //}

        //public static bool operator ==(_ULong a, _ULong b)
        //{
        //    return a.Value == b.Value;
        //}

        //public static bool operator !=(_ULong a, _ULong b)
        //{
        //    return a.Value != b.Value;
        //}

        //public static _ULong operator ++(_ULong s)
        //{
        //    return new _ULong(s.Value + 1);
        //}

        //public static _ULong operator --(_ULong s)
        //{
        //    return new _ULong(s.Value - 1);
        //}
    }
}
