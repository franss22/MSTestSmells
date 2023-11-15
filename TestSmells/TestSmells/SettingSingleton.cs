using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;



namespace TestSmells
{
    public class SettingSingleton
    {
        //Custom Settings provided by the user
        private bool CustomSettingsProvided;
        //If enabled, settings not set by CustomSettings will be null. Otherwise, GetSettings will try to look in AnalyzerConfigOptions
        private bool HardOverride;


        static SettingSingleton Instance;

        Dictionary<string, string> Settings;


        private SettingSingleton()
        {
            CustomSettingsProvided = false;
            HardOverride = false;
        }

        public static SettingSingleton GetInstance()
        {
            if (Instance == null)
            {
                Instance = new SettingSingleton();
            }
            return Instance;
        }

        public static void SetSettings(Dictionary<string, string> newSettings, bool _delegate)
        {
            var S = GetInstance();
            S.Settings = newSettings;
            S.CustomSettingsProvided = true;
            S.HardOverride = _delegate;
        }

        public static string GetSettings(AnalyzerConfigOptions defaultOptions, string key) 
        {
            var S = GetInstance();
            string settingValue;
            if (S.CustomSettingsProvided)
            {
                if (S.Settings.TryGetValue(key, out settingValue))
                {
                    return settingValue;
                }
                else if (!S.HardOverride)
                {
                    defaultOptions.TryGetValue(key, out settingValue);
                    return settingValue;
                }
                else return null;
            }
            else
            {
                defaultOptions.TryGetValue(key, out settingValue);
                return settingValue;
            }
        }

    }
}
