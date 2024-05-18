using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.AspNetCore;

namespace ERPSystem.Common.Infrastructure
{
    public static class EnumHelper
    {

        public static List<EnumModel> ToEnumList<T>()
        {
            List<EnumModel> enumModels = new List<EnumModel>();
            Type t = typeof(T);
            if (t.IsEnum)
            {
                var values = Enum.GetValues(t).Cast<Enum>().Select(e => new
                {
                    Id = Convert.ToInt32(e),
                    Name = e.GetDescription()
                });

                foreach (var item in values)
                {
                    enumModels.Add(new EnumModel { Id = item.Id, Name = item.Name });
                }
            }
            return enumModels;
        }

        public static List<EnumModelWithValue> ToEnumListWithValue<T>()
        {
            List<EnumModelWithValue> enumModels = new List<EnumModelWithValue>();
            Type t = typeof(T);
            if (t.IsEnum)
            {
                var values = Enum.GetValues(t).Cast<Enum>().Select(e => new
                {
                    Id = Convert.ToInt32(e),
                    Name = e.GetDescription(),
                    Value = e.GetName()
                });

                foreach (var item in values)
                {
                    enumModels.Add(new EnumModelWithValue { Id = item.Id, Name = item.Name, Value = item.Value });
                }
            }
            return enumModels;
        }

        public static List<dynamic> ToEnumListText<T>()
        {
            List<dynamic> enumModels = new List<dynamic>();
            Type t = typeof(T);
            if (t.IsEnum)
            {
                var values = Enum.GetValues(t).Cast<Enum>().Select(e => new
                {
                    Id = e.GetDescription(),
                    Name = e.GetDescription()
                });

                foreach (var item in values)
                {
                    enumModels.Add(new {item.Id, item.Name });
                }
            }
            return enumModels;
        }

        public static string GetDescription<TEnum>(this TEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi != null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
            }
            return value.ToString();
        }

        public static List<string> GetAllDescriptions<TEnum>(int value, ResourceManager resourceManager)
        {
            try
            {
                var enumName = Enum.GetName(typeof(TEnum), value);
                if (enumName == null)
                {
                    return new List<string>(value);
                }
                else if(resourceManager.GetString($"lbl{enumName}") != null)
                {
                    List<string> values = new List<string>();
                    List<string> languages = new List<string>() { "en-us", "ja-jp", "ko-kr", "vi-vn" };
                    foreach (var language in languages)
                    {
                        values.Add(resourceManager.GetString($"lbl{enumName}", new CultureInfo(language)));
                    }

                    return values;
                }
                else
                {
                    return new List<string>();
                }
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }

        public static string GetName<TEnum>(this TEnum value)
        {
            try
            {
                return value.GetType().GetMember(value.ToString()).First().Name;
            }
            catch (Exception e)
            {
                return "";
            }
        }
       

        public static int GetValueByName(Type type, string name)
        {
            try
            {
                int value = (int)Enum.Parse(type, name);
                return value;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        
        public static Dictionary<string, List<string>> GetAllDescriptionWithEnum<T>(ResourceManager resourceManager)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            
            Type t = typeof(T);
            var names = Enum.GetNames(t);

            foreach (string name in names)
            {
                List<string> values = GetAllDescriptions<T>((int)Enum.Parse(t, name), resourceManager);
                result.Add(name, values);
            }

            return result;
        }
    }
    public class EnumModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class EnumModelWithValue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class EnumModelRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }

    public class AutoComplete
    {
        public int Value { get; set; }
        public string Label { get; set; }
    }
}
