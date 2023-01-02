using UnityEngine;

namespace PhEngine.Core
{
    public class PhLogger : Singleton<PhLogger>
    {
        [SerializeField] LoggerConfig config;
        
        internal static bool IsEnabled => IsCannotGetConfig() || Instance.config.IsEnabled;
        internal static bool IsShowTag => IsCannotGetConfig() || Instance.config.IsShowTag;
        static bool IsCannotGetConfig()
        {
            return Instance == null
                   || Instance.config == null;
        }
        
        internal static LogSetting GetSetting(string tagName)
        {
            if (IsCannotGetSetting(tagName))
                return new LogSetting();

            return Instance.config.GetSettingByTag(tagName);
        }

        static bool IsCannotGetSetting(string tagName)
        {
            return IsCannotGetConfig()
                   || string.IsNullOrEmpty(tagName);
        }

        protected override void InitAfterAwake()
        {
        }
    }
}