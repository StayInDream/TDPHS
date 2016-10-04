using System;

namespace Spate
{
    /// <summary>
    /// 安全类型short
    /// </summary>
    public struct _Short
    {
        private static Random rnd;
        static _Short()
        {
            rnd = new Random(Environment.TickCount);
        }

        private short encValue;
        private byte xorKey;

        public _Short(short defValue = 0)
        {
            encValue = 0;
            xorKey = 0;
            Value = defValue;
        }

        public short Value
        {
            get
            {
                return (short)(xorKey ^ encValue);
            }
            set
            {
                xorKey = (byte)rnd.Next(0, byte.MaxValue);
                encValue = (short)(xorKey ^ value);
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is _Short) && ((_Short)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        //public static implicit operator short(_Short s)
        //{
        //    return s.Value;
        //}

        //public static implicit operator _Short(sbyte v)
        //{
        //    return new _Short(v);
        //}

        //public static implicit operator _Short(byte v)
        //{
        //    return new _Short(v);
        //}

        //public static implicit operator _Short(short v)
        //{
        //    return new _Short(v);
        //}

        //public static bool operator ==(_Short a, _Short b)
        //{
        //    return a.Value == b.Value;
        //}

        //public static bool operator !=(_Short a, _Short b)
        //{
        //    return a.Value != b.Value;
        //}

        //public static _Short operator ++(_Short s)
        //{
        //    return new _Short((short)(s.Value + 1));
        //}

        //public static _Short operator --(_Short s)
        //{
        //    return new _Short((short)(s.Value - 1));
        //}
    }
}