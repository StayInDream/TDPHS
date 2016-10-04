using System;

namespace Spate
{
    /// <summary>
    /// 安全类型int
    /// </summary>
    public struct _Int
    {
        private static Random rnd;
        static _Int()
        {
            rnd = new Random(Environment.TickCount);
        }

        private int encValue;
        private byte xorKey;

        public _Int(int defValue = 0)
        {
            encValue = 0;
            xorKey = 0;
            Value = defValue;
        }

        public int Value
        {
            get
            {
                // 如果从未赋值过则保持默认值
                return xorKey ^ encValue;
            }
            set
            {
                xorKey = (byte)rnd.Next(1, byte.MaxValue);
                encValue = xorKey ^ value;
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is _Int) && ((_Int)obj).Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        //// int xxx = _intVal; _Int自动转换成int
        //public static implicit operator int(_Int s)
        //{
        //    return s.Value;
        //}
        //// _intVal = (sbyte)100;
        //public static implicit operator _Int(sbyte v)
        //{
        //    return new _Int(v);
        //}
        //// _intVal = (byte)100;
        //public static implicit operator _Int(byte v)
        //{
        //    return new _Int(v);
        //}
        //// _intVal = (short)100;
        //public static implicit operator _Int(short v)
        //{
        //    return new _Int(v);
        //}
        //// _intVal = (ushort)100;
        //public static implicit operator _Int(ushort v)
        //{
        //    return new _Int(v);
        //}
        //// _intVal = 100; int自动转换成类型_Int
        //public static implicit operator _Int(int v)
        //{
        //    return new _Int(v);
        //}
        //// if(_intVal == 100)
        //public static bool operator ==(_Int a, _Int b)
        //{
        //    return a.Value == b.Value;
        //}
        //// if(_intVal != 100)
        //public static bool operator !=(_Int a, _Int b)
        //{
        //    return a.Value != b.Value;
        //}
        //// _intVal++;
        //public static _Int operator ++(_Int s)
        //{
        //    return new _Int(s.Value + 1);
        //}
        //// _intVal--
        //public static _Int operator --(_Int s)
        //{
        //    return new _Int(s.Value - 1);
        //}
    }
}
