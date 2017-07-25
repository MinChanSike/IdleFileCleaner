using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace IdleFileCleaner {
    public class ConfigHelper {
        private static Dictionary<string, string> _appSettings;
        public static void Init(string config) {
            _appSettings = new Regex("(\\w+?)=('.+?')")
              .Matches(config)
              .Cast<Match>()
              //.Skip(1)
              .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value.Replace("'", ""));
        }
        public static string Read(string key) {
            return Read<string>(key);
        }
        public static T Read<T>(string key) {
            string configValue = GetConfigValue(key);
            if (configValue != null) {
                var data = (T)Convert.ChangeType(configValue, typeof(T));
                return data;
            }
            throw new ArgumentException(key + " is not found in the config.");
        }
        public static T ReadOrDefault<T>(string key) {
            return ReadOrDefault(key, default(T));
        }
        public static T ReadOrDefault<T>(string key, T defaultValue) {
            try {
                var configValue = GetConfigValue(key);
                if (configValue != null) {
                    var data = (T)Convert.ChangeType(configValue, typeof(T));
                    return data;
                }
            } catch (Exception ex) {
                Trace.WriteLine(String.Format("An error on reading value for key {0}:{1}", key, ex));
            }
            return defaultValue;
        }
        private static string GetConfigValue(string key) {
            if (_appSettings != null && _appSettings.ContainsKey(key)) {
                return _appSettings[key];
            }
            return ConfigurationManager.AppSettings[key];
        }
        public static void Write<T>(string key, T value) {
            System.Configuration.Configuration config =
            ConfigurationManager.OpenExeConfiguration
                    (ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null) {
                config.AppSettings.Settings.Add(key, value.ToString());
            } else {
                config.AppSettings.Settings[key].Value = value.ToString();
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

    }
}
