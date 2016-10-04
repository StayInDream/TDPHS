using System;

namespace Spate
{
    /// <summary>
    /// 安全类型byte
    /// </summary>
    public struct _SByte
    {
        private static Random rnd;
        static _SByte()
        {
            rnd = new Random(Environment.TickCount);
        }

        private sbyte encValue;
        private byte xorKey;

        public _SByte(sbyte defValue = 0)
        {
            encValue = 0;
            xorKey = 0;
            Value = defValue;
        }

        public sbyte Value
        {
            get
            {
                return (sbyte)(xorKey ^ encValue);
            }
            set
            {
                xorKey = (byte)rnd.Next(0, byte.MaxValue);
                encValue = (sbyte)(xorKey ^ value);
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is _SByte) && ((_SByte)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        //public static implicit operator int(_SByte s)
        //{
        //    return s.Value;
        //}

        //public static implicit operator _SByte(sbyte v)
        //{
        //    return new _SByte(v);
        //}

        //public static bool operator ==(_SByte a, _SByte b)
        //{
        //    return a.Value == b.Value;
        //}

        //public static bool operator !=(_SByte a, _SByte b)
        //{
        //    return a != b;
        //}

        //public static _SByte operator ++(_SByte s)
        //{
        //    return new _SByte((sbyte)(s.Value + 1));
        //}

        //public static _SByte operator --(_SByte s)
        //{
        //    return new _SByte((sbyte)(s.Value - 1));
        //}
    }
}