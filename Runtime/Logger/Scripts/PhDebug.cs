using UnityEngine;

namespace PhEngine.Core
{
    public static class PhDebug
    {
        const string OPEN_BRACKET = "<b>[";
        const string CLOSE_BRACKET = "]</b> ";
        static readonly Color FadedColor = new Color(1,1,1,0.2f);

        #region Logging
        
        public static void Log(string tag, string message, LogType type = LogType.Log)
        {
            if (!PhLogger.IsEnabled)
                return;

            var loggerSetting = PhLogger.GetSetting(tag);
            ForceLog(tag, message, type, loggerSetting);
        }

        static void ForceLog(string tag, string message, LogType type, LogSetting logSetting)
        {
            if (logSetting.displayOption == LogDisplayOption.Hide)
                return;
            
            var finalMessage = CreateDisplayMessage(tag, message, logSetting);
            LogToConsole(type, finalMessage);
        }
        
        static string CreateDisplayMessage(string tag, string message, LogSetting logSetting)
        {
            tag = TryGetTagWithBrackets(tag);
            if (logSetting.displayOption == LogDisplayOption.Faded)
                return GetColorizedMessage($"{tag}{message}", FadedColor);

            return $"{GetColorizedMessage(tag, logSetting.color)}{message}";
        }

        public static string GetColorizedMessage(string message, Color color)
        {
            var colorHexString = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{colorHexString}>{message}</color>";
        }

        static string TryGetTagWithBrackets(string tag)
        {
            return IsCannotGetTag(tag) ? string.Empty : $"{OPEN_BRACKET}{tag}{CLOSE_BRACKET} ";
        }

        static bool IsCannotGetTag(string tag)
        {
            return string.IsNullOrEmpty(tag) 
                   || !PhLogger.IsShowTag;
        }

        static void LogToConsole(LogType logType, string finalMessage)
        {
            switch (logType)
            {
                case LogType.Log:
                    Debug.Log(finalMessage);
                    break;

                case LogType.Error:
                    Debug.LogError(finalMessage);
                    break;

                case LogType.Warning:
                    Debug.LogWarning(finalMessage);
                    break;
            }
        }
        
        #endregion

        #region Logging Helpers
        
        public static void Log<T>(string message)
        {
            Log(typeof(T).Name, message);
        }

        public static void LogWarning<T>(string message)
        {
            Log(typeof(T).Name, message, LogType.Warning);
        }

        public static void LogError<T>(string message)
        {
            Log(typeof(T).Name, message, LogType.Error);
        }

        public static void LogWarning(string tag, string message)
        {
            Log(tag, message, LogType.Warning);
        }

        public static void LogError(string tag, string message)
        {
            Log(tag, message, LogType.Error);
        }

        public static void LogWarning(string message)
        {
            Log(string.Empty, message, LogType.Warning);
        }

        public static void LogError(string message)
        {
            Log(string.Empty, message, LogType.Error);
        }
        
        #endregion
    }
}