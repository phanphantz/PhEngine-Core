using System;
using System.Reflection;

namespace PhEngine.Core.JSON
{
    public class JSONFieldName : Attribute  
    {  
        public string name;
        public JSONFieldName(string name)  
        {  
            this.name = name;
        }  
        
        public static string Find(FieldInfo fieldInfo)
        {
            var jsonFieldNameAttributes = GetFieldNameAttributes(fieldInfo);
            return jsonFieldNameAttributes.Length == 0 ? 
                fieldInfo.Name : 
                (jsonFieldNameAttributes[0] as JSONFieldName)?.name;
        }

        static object[] GetFieldNameAttributes(FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttributes(typeof(JSONFieldName), true);
        }
    }
}