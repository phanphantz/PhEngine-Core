using System;
using UnityEngine;

namespace PhEngine.Core
{
    [Serializable]
    public class LogSetting
    {
        public string tag;
        public LogDisplayOption displayOption;
        public Color color = Color.white;
    }
}