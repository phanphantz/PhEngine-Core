using System;

namespace PhEngine.Core.JSON
{
    [Serializable]
    public abstract class JSONConvertibleObject 
    {
        public virtual JSONObject CreateJSON()
        {
            return JSONConverter.From(this);
        }

        public virtual void SetValueByJSON(JSONObject json)
        {
            JSONConverter.SetValueByJSON(this, json);
        }
    }
}