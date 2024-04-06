using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns.Tools
{
    public static class EnumUtil
    {
        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetDesc(Enum obj)
        {
            string objName = obj.ToString();
            Type t = obj.GetType();
            FieldInfo fi = t.GetField(objName);
            if (fi == null)
                return objName;
            DescriptionAttribute[] arrDesc = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (arrDesc.Length > 0)
                return arrDesc[0].Description;
            else
                return objName;
        }
        /// <summary>
        /// 获取枚举的键值对集合
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetEnumIntNameDict(Enum e)
        {
            return GetEnumNameValueDict(e, (Object o) => { return Convert.ToInt32(o); });
        }
        public static Dictionary<TOutput, string> GetEnumNameValueDict<TOutput>(Enum e, Converter<Object, TOutput> convert)
        {
            Type enumType = e.GetType();
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("参数必须是枚举类型", "enumType");
            }

            FieldInfo[] fields = enumType.GetFields();

            if (fields != null)
            {
                Dictionary<TOutput, string> dict = new Dictionary<TOutput, string>();

                foreach (FieldInfo f in fields)
                {
                    if (f.Name.EndsWith("__")) continue;

                    DescriptionAttribute[] attrs = (DescriptionAttribute[])f.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    string name = f.Name;
                    if (attrs != null && attrs.Length == 1)
                    {
                        name = attrs[0].Description;
                    }
                    TOutput val = convert(f.GetValue(e));
                    dict.Add(val, name);
                }
                return dict;
            }

            return null;
        }
        /// <summary>
        /// 获取枚举的键值对集合
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEnumVarNameDict(Enum e)
        {
            Type enumType = e.GetType();
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("参数必须是枚举类型", "enumType");
            }

            FieldInfo[] fields = enumType.GetFields();

            if (fields != null)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();

                foreach (FieldInfo f in fields)
                {
                    if (f.Name.EndsWith("__")) continue;

                    DescriptionAttribute[] attrs = (DescriptionAttribute[])f.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    string name = f.Name;
                    if (attrs != null && attrs.Length == 1)
                    {
                        name = attrs[0].Description;
                    }
                    dict.Add(f.Name, name);
                }
                return dict;
            }

            return null;
        }

        private static SortedList<Enum, string> enumDesciptionDict = new SortedList<Enum, string>();
        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            string description = string.Empty;
            if (enumDesciptionDict.TryGetValue(value, out description)) return description;

            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute == null)
                enumDesciptionDict[value] = value.ToString();
            else
                enumDesciptionDict[value] = attribute.Description;
            return enumDesciptionDict[value];
        }
        /// <summary>
        /// 获取特性
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DescriptionAttribute GetAttribute(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            return Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        }
    }
}

