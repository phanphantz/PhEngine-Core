using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PhEngine.Core.JSON
{
    public static class JSONDictionaryUtils
    {
        public static Dictionary<string, object> CreateValueDictionary(object obj)
        {
            var fields = GetAllFields(obj);
            return fields?.ToDictionary(JSONFieldName.Find, f => f.GetValue(obj));
        }

        public static Dictionary<FieldInfo, Type> CreateFieldDictionary(object obj)
        {
            var fields = GetAllFields(obj);
            return fields?.ToDictionary(f=> f, f => f.FieldType);
        }
        
        static FieldInfo[] GetAllFields(object obj)
        {
            return obj?.GetType().GetFields();
        }
    }
}