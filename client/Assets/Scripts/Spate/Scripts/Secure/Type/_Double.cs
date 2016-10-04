using System;

namespace Spate
{
    /// <summary>
    /// 安全类型double
    /// </summary>
    public struct _Double
    {
        private static Random rnd;
        static _Double()
        {
            rnd = new Random(Environment.TickCount);
        }

        private ulong encValue;
        private byte xorKey;

        public _Double(double defValue = 0)
        {
            encValue = 0;
            xorKey = 0;
            Value = defValue;
        }

        public double Value
        {
            get
            {
                byte[] raw = BitConverter.GetBytes(encValue);
                for (int i = 0; i < raw.Length; i++)
                {
                    raw[i] ^= xorKey;
                }
                return BitConverter.ToDouble(raw, 0);
            }
            set
            {
                xorKey = (byte)rnd.Next(0, byte.MaxValue);

                byte[] raw = BitConverter.GetBytes(value);
                for (int i = 0; i < raw.Length; i++)
                {
                    raw[i] ^= xorKey;
                }
                encValue = BitConverter.ToUInt64(raw, 0);
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is _Double) && ((_Double)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        //public static implicit operator double(_Double s)
        //{
        //    return s.Value;
        //}

        //public static implicit operator _Double(sbyte v)
        //{
        //    return new _Double(v);
        //}

        //public static implicit operator _Double(byte v)
        //{
        //    return new _Double(v);
        //}

        //public static implicit operator _Double(short v)
        //{
        //    return new _Double(v);
        //}

        //public static implicit operator _Double(ushort v)
        //{
        //    return new _Double(v);
        //}

        //public static implicit operator _Double(int v)
        //{
        //    return new _Double(v);
        //}

        //public static implicit operator _Double(uint v)
        //{
        //    return new _Double(v);
        //}

        //public static implicit operator _Double(float v)
        //{
        //    return new _Double(v);
        //}

        //public static implicit operator _Double(long v)
        //{
        //    return new _Double(v);
        //}

        //public static implicit operator _Double(ulong v)
        //{
        //    return new _Double(v);
        //}

        //public static implicit operator _Double(double v)
        //{
        //    return new _Double(v);
        //}

        //public static bool operator ==(_Double a, _Double b)
        //{
        //    return a.Value == b.Value;
        //}

        //public static bool operator !=(_Double a, _Double b)
        //{
        //    return a != b;
        //}

        //public static _Double operator ++(_Double s)
        //{
        //    return new _Double(s.Value + 1);
        //}

        //public static _Double operator --(_Double s)
        //{
        //    return new _Double(s.Value - 1);
        //}
    }
}