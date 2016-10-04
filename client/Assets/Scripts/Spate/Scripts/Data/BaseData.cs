using System;
using System.Reflection;
using System.Collections.Generic;

namespace Spate
{
    /// <summary>
    /// Data的基类,自动映射,需要提供主键信息,如果想捕获字段被更新的消息,
    /// 请重写OnInitNotify并注册字段和消息码映射
    /// </summary>
    public abstract class BaseData
    {
        /// <summary>
        /// 获取主键值,便于DataManager的Table进行存储和索引,如果无主键,就继承BaseNoKeyData
        /// </summary>
        public abstract object PrimaryKey { get; }

        /// <summary>
        /// 数据发生改变时回调
        /// </summary>
        public virtual void OnDataUpdate(string name, object newValue, object oldValue)
        {

        }
        /// <summary>
        /// 数据发生改变时回调,每个对象仅会调用一次
        /// </summary>
        public virtual void OnDataUpdate(List<string> names, List<object> newValues, List<object> oldValues)
        {

        }

        public virtual void OnDataDelete()
        {

        }

        public virtual void OnDataAdd()
        {

        }


        public void CopyFrom(params BaseData[] newDatas)
        {
            foreach (BaseData newData in newDatas)
            {
                CopyFrom(newData);
            }
        }

        public void CopyFrom(BaseData newData)
        {
            if (newData == null)
                return;
            // 筛选出自身的有效字段/属性集合
            Dictionary<string, MemberInfo> myFieldMap = new Dictionary<string, MemberInfo>();
            // 获取全部字段
            {
                FieldInfo[] arr = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (FieldInfo f in arr)
                {
                    myFieldMap.Add(f.Name, f);
                }
            }
            // 获取全部属性
            {
                PropertyInfo[] arr = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (PropertyInfo f in arr)
                {
                    if (f.Name.Equals("PrimaryKey"))
                        continue;
                    myFieldMap.Add(f.Name, f);
                }
            }

            //Dictionary<string, FieldInfo> myFieldMap = new Dictionary<string, FieldInfo>();
            //FieldInfo[] myfieldArray = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            //foreach (FieldInfo f in myfieldArray)
            //{
            //    myFieldMap.Add(f.Name, f);
            //}

            // 获取newData所有有效的字段并取值
            if (newData == null)
                MLogger.Log("new Data is Null");


            Dictionary<string, object> newDataFieldValueMap = new Dictionary<string, object>();
            List<MemberInfo> newFieldArray = new List<MemberInfo>();
            newFieldArray.AddRange(newData.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            newFieldArray.AddRange(newData.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            //FieldInfo[] newFieldArray = newData.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);


            bool acrossType = !GetType().Equals(newData.GetType());
            foreach (MemberInfo f in newFieldArray)
            {
                string name = f.Name;
                if ("PrimaryKey".Equals(name))
                    continue;
                if (acrossType)
                {
                    // 对于重定向字段,我们不仅仅要更新重定向后的字段,还要更新同名字段
                    name = GetFieldName(f);
                    if (string.Equals(name, f.Name))
                    {
                        if (myFieldMap.ContainsKey(f.Name))
                        {
                            // object value = f.GetValue(newData);
                            object value = GetMemberValue(f, newData);
                            newDataFieldValueMap.Add(f.Name, value);
                        }
                    }
                    else
                    {
                        // f被应用了FieldRedirect属性,需要更新重定向后的字段和同名字段(这样同名字段就意味着保留上一次的值),便于下次判定
                        //if (myFieldMap.ContainsKey(f.Name))
                        //{
                        //    object value = f.GetValue(newData);
                        //    // PlayerData.CopyFrom(PlayerSvrData)
                        //    // 如果PlayerData.atk和PlayerSvrData.atk相同,就不用拷贝
                        //    object xxx_usrData = myFieldMap[f.Name].GetValue(this);
                        //    // 判断数值采用hashCode会比较快捷
                        //    if (value.GetHashCode() == xxx_usrData.GetHashCode() &&
                        //        !string.Equals("0", myFieldMap[name].GetValue(this).ToString()))
                        //    {
                        //        continue;
                        //    }

                        //    newDataFieldValueMap.Add(f.Name, value);
                        //}
                        if (myFieldMap.ContainsKey(name))
                        {
                            // object value = f.GetValue(newData);
                            object value = GetMemberValue(f, newData);
                            newDataFieldValueMap.Add(name, value);
                        }
                    }
                }
                else
                {
                    // 对于不跨类型拷贝,直接拷贝
                    if (myFieldMap.ContainsKey(f.Name))
                    {
                        // object value = f.GetValue(newData);
                        object value = GetMemberValue(f, newData);
                        newDataFieldValueMap.Add(f.Name, value);
                    }
                }
            }

            // 拷贝值
            foreach (KeyValuePair<string, MemberInfo> pair in myFieldMap)
            {
                string name = pair.Key;
                if (newDataFieldValueMap.ContainsKey(name))
                {
                    object value = newDataFieldValueMap[name];
                    // 覆盖值
                    try
                    {
                        if (value == null)
                        {
                            SetMemberValue(pair.Value, this, null);
                            //pair.Value.SetValue(this, null);
                        }
                        else
                        {
                            // 引用类型会直接拷贝引用过去，会在修改this的时候直接引发newData也被修改
                            Type t = value.GetType();
                            if (t.IsArray)
                            {
                                Type eType = t.GetElementType();
                                Array srcArray = (Array)value;
                                Array array = Array.CreateInstance(eType, srcArray.Length);
                                //pair.Value.SetValue(this, array);
                                SetMemberValue(pair.Value, this, array);
                                for (int i = 0, len = srcArray.Length; i < len; i++)
                                {
                                    array.SetValue(srcArray.GetValue(i), i);
                                }
                            }
                            else
                            {
                                SetMemberValue(pair.Value, this, value);
                                // pair.Value.SetValue(this, value);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MLogger.LogException(ex);
                        MLogger.LogError(string.Format("{0}.{1}", this.GetType().Name, name));
                    }
                }
            }
        }

        public static void SetMemberValue(MemberInfo info, object obj, object value)
        {
            if (info is FieldInfo)
                ((FieldInfo)info).SetValue(obj, value);
            else
                ((PropertyInfo)info).SetValue(obj, value, null);
        }

        public static object GetMemberValue(MemberInfo info, object obj)
        {
            if (info is FieldInfo)
                return ((FieldInfo)info).GetValue(obj);
            else
                return ((PropertyInfo)info).GetValue(obj, null);
        }

        public static Type GetMemberType(MemberInfo info)
        {
            if (info is FieldInfo)
                return ((FieldInfo)info).FieldType;
            else
                return ((PropertyInfo)info).PropertyType;
        }

        private string GetFieldName(MemberInfo field)
        {
            string name = field.Name;
            // 属性判定
            FieldRedirectAttribute[] arr = field.GetCustomAttributes(typeof(FieldRedirectAttribute), false) as FieldRedirectAttribute[];
            if (arr != null && arr.Length > 0)
            {
                FieldRedirectAttribute attr = arr[0];
                name = attr.Value;
            }
            return name;
        }

        private MemberInfo mPrimaryKey;
        public MemberInfo GetPrimaryKey()
        {
            if (mPrimaryKey == null)
            {
                FieldInfo[] fieldArray = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (FieldInfo f in fieldArray)
                {
                    object[] attrs = f.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        mPrimaryKey = f;
                    }
                }
            }
            if (mPrimaryKey == null)
            {
                PropertyInfo[] fieldArray = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (PropertyInfo f in fieldArray)
                {
                    object[] attrs = f.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        mPrimaryKey = f;
                    }
                }
            }
            return mPrimaryKey;
        }

        private MemberInfo mCsvForeginKey;
        public MemberInfo GetCsvForeginKey()
        {
            if (mCsvForeginKey == null)
            {
                FieldInfo[] fieldArray = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (FieldInfo f in fieldArray)
                {
                    object[] attrs = f.GetCustomAttributes(typeof(CsvForeginKeyAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        mCsvForeginKey = f;
                    }
                }
            }
            if (mCsvForeginKey == null)
            {
                PropertyInfo[] fieldArray = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (PropertyInfo f in fieldArray)
                {
                    object[] attrs = f.GetCustomAttributes(typeof(CsvForeginKeyAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        mCsvForeginKey = f;
                    }
                }
            }
            return mCsvForeginKey;
        }

    }
}
