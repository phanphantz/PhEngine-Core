using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PhEngine.Core
{
    [CreateAssetMenu(menuName = "PhEngine/Debugging/LoggerConfig", fileName = "LoggerConfig", order = 0)]
    public class LoggerConfig : ScriptableObject
    {
        [SerializeField] bool isEnabled = true;
        public bool IsEnabled => isEnabled;

        [SerializeField] bool isShowTag = true;
        public bool IsShowTag => isShowTag;

        [SerializeField] List<LogSetting> logSettingList = new List<LogSetting>();

        public LogSetting GetSettingByTag(string tag)
        {
            var result = logSettingList.FirstOrDefault(setting => setting.tag == tag);
            return result ?? new LogSetting();
        }
    }
}