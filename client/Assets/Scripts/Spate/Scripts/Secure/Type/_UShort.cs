using System;

namespace Spate
{
    /// <summary>
    /// 安全类型ushort
    /// </summary>
    public struct _UShort
    {
        private static Random rnd;
        static _UShort()
        {
            rnd = new Random(Environment.TickCount);
        }

        private ushort encValue;
        private byte xorKey;

        public _UShort(ushort defValue = 0)
        {
            encValue = 0;
            xorKey = 0;
            Value = defValue;
        }

        public ushort Value
        {
            get
            {
                return (ushort)(xorKey ^ encValue);
            }
            set
            {
                xorKey = (byte)rnd.Next(0, byte.MaxValue);
                encValue = (ushort)(xorKey ^ value);
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is _UShort) && ((_UShort)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        //public static implicit operator ushort(_UShort s)
        //{
        //    return s.Value;
        //}

        //public static implicit operator _UShort(byte v)
        //{
        //    return new _UShort(v);
        //}

        //public static implicit operator _UShort(ushort v)
        //{
        //    return new _UShort(v);
        //}

        //public static bool operator ==(_UShort a, _UShort b)
        //{
        //    return a.Value == b.Value;
        //}

        //public static bool operator !=(_UShort a, _UShort b)
        //{
        //    return a.Value != b.Value;
        //}

        //public static _UShort operator ++(_UShort s)
        //{
        //    return new _UShort((ushort)(s.Value + 1));
        //}

        //public static _UShort operator --(_UShort s)
        //{
        //    return new _UShort((ushort)(s.Value - 1));
        //}
    }
}
