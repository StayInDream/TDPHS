using System;

namespace Spate
{
    /// <summary>
    /// 安全类型long
    /// </summary>
    public struct _Long
    {
        private static Random rnd;
        static _Long()
        {
            rnd = new Random(Environment.TickCount);
        }

        private long encValue;
        private byte xorKey;

        public _Long(long defValue = 0)
        {
            encValue = 0;
            xorKey = 0;
            Value = defValue;
        }

        public long Value
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
            return (obj is _Long) && ((_Long)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        //public static implicit operator long(_Long s)
        //{
        //    return s.Value;
        //}

        //public static implicit operator _Long(sbyte v)
        //{
        //    return new _Long(v);
        //}

        //public static implicit operator _Long(byte v)
        //{
        //    return new _Long(v);
        //}

        //public static implicit operator _Long(short v)
        //{
        //    return new _Long(v);
        //}

        //public static implicit operator _Long(ushort v)
        //{
        //    return new _Long(v);
        //}

        //public static implicit operator _Long(int v)
        //{
        //    return new _Long(v);
        //}

        //public static implicit operator _Long(uint v)
        //{
        //    return new _Long(v);
        //}

        //public static implicit operator _Long(long v)
        //{
        //    return new _Long(v);
        //}

        //public static bool operator ==(_Long a, _Long b)
        //{
        //    return a.Value == b.Value;
        //}

        //public static bool operator !=(_Long a, _Long b)
        //{
        //    return a.Value != b.Value;
        //}

        //public static _Long operator ++(_Long s)
        //{
        //    return new _Long(s.Value + 1);
        //}

        //public static _Long operator --(_Long s)
        //{
        //    return new _Long(s.Value - 1);
        //}
    }
}