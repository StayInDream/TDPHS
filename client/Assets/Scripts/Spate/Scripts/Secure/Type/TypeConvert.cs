using System;
using System.Reflection;

namespace Spate
{
    public static class TypeConvert
    {
        private const string PREFIX = "Spate._";
        //public static object ChangeType(object value, Type conversionType)
        //{
        //    string target_full_type_name = conversionType.FullName;
        //    string value_full_type_name = value.GetType().FullName;
        //    bool target_type_is_user_def = target_full_type_name.StartsWith(PREFIX);
        //    bool value_type_is_user_def = value_full_type_name.StartsWith(PREFIX);

        //    object result = null;
        //    if (target_type_is_user_def && !value_type_is_user_def)
        //    {
        //        // 需要把value转换成自定义类型,比如int->_Int
        //        result = Activator.CreateInstance(conversionType, value);
        //    }
        //    else if (!target_type_is_user_def && value_type_is_user_def)
        //    {
        //        // 需要获取value的内置类型值,获取value属性值
        //        Type valueType = value.GetType();
        //        PropertyInfo property = valueType.GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        //        result = property.GetValue(value, null);
        //    }
        //    else
        //    {
        //        result = Convert.ChangeType(value, conversionType);
        //    }
        //    return result;
        //}
    }
}
