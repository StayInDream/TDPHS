using System;

namespace Spate
{
    /// <summary>
    /// 安全类型byte
    /// </summary>
    public struct _Byte
    {
        private static Random rnd;
        static _Byte()
        {
            rnd = new Random(Environment.TickCount);
        }

        private byte encValue;
        private byte xorKey;

        public _Byte(byte defValue = 0)
        {
            encValue = 0;
            xorKey = 0;
            Value = defValue;
        }

        public byte Value
        {
            get
            {
                return (byte)(xorKey ^ encValue);
            }
            set
            {
                xorKey = (byte)rnd.Next(0, byte.MaxValue);
                encValue = (byte)(xorKey ^ value);
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is _Byte) && ((_Byte)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        //public static implicit operator int(_Byte s)
        //{
        //    return s.Value;
        //}

        //public static implicit operator _Byte(byte v)
        //{
        //    return new _Byte(v);
        //}

        //public static bool operator ==(_Byte a, _Byte b)
        //{
        //    return a.Value == b.Value;
        //}

        //public static bool operator !=(_Byte a, _Byte b)
        //{
        //    return a != b;
        //}

        //public static _Byte operator ++(_Byte s)
        //{
        //    return new _Byte((byte)(s.Value + 1));
        //}

        //public static _Byte operator --(_Byte s)
        //{
        //    return new _Byte((byte)(s.Value - 1));
        //}
    }
}