using System;
using System.Collections.Generic;
using System.IO;
using Analytics.Scripts.Analytics;
using UnityEngine;

namespace Analytics.Scripts.LocalStorage
{
    public class FileLocalStorage
    {
        private static string EXTENSION = ".evt";
        
        #region Primitive singleton

        private static FileLocalStorage _instance;
        public static FileLocalStorage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FileLocalStorage();
                }

                return _instance;
            }
        }

        #endregion


        public Action<string> OnEventAdded;
        public Action OnEventsCleared;

        public void AddAnalyticsEvent(AnalyticsEvent analyticsEvent)
        {
            // Ensure one event per file
            int counter = -1;
            string path;
            do
            {
                counter += 1;
                path = Application.persistentDataPath + "/" + analyticsEvent.timestamp + "_" + counter + EXTENSION;
            } while (File.Exists(path));

            StreamWriter writer = new StreamWriter(path);
            var analyticsEventStr = AnalyticsEvent.Serialize(analyticsEvent);
            writer.WriteLine(analyticsEventStr);
            writer.Close();
            
            OnEventAdded?.Invoke(analyticsEventStr);
        }

        public List<AnalyticsEvent> GetAnalyticsEvents(long from, long to)
        {
            var analyticsEvents = new List<AnalyticsEvent>();

            var searchPattern = $"*{EXTENSION}";
            foreach (var filePath in Directory.EnumerateFiles(Application.persistentDataPath, searchPattern))
            {
                var timestampStr = filePath.Substring(filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                timestampStr = timestampStr.Replace(EXTENSION, "");
                var timestampAndCounter = timestampStr.Split('_');
                var timestamp = long.Parse(timestampAndCounter[0]);
                
                // Skip events outside of from-to interval
                if (timestamp < from || timestamp > to)
                    continue;

                StreamReader reader = new StreamReader(filePath);
                
                var analyticsEventStr = reader.ReadToEnd();
                var analyticsEvent = AnalyticsEvent.Deserialize(analyticsEventStr);
                analyticsEvents.Add(analyticsEvent);

                reader.Close();
            }

            return analyticsEvents;
        }

        public void ClearAnalyticsEvents(long from, long to)
        {
            var searchPattern = $"*{EXTENSION}";
            foreach (var filePath in Directory.EnumerateFiles(Application.persistentDataPath, searchPattern))
            {
                var timestampStr = filePath.Substring(filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                timestampStr = timestampStr.Replace(EXTENSION, "");
                var timestampAndCounter = timestampStr.Split('_');
                var timestamp = long.Parse(timestampAndCounter[0]);
                
                // Skip events outside of from-to interval
                if (timestamp < from || timestamp > to)
                    continue;

                File.Delete(filePath);
            }
            
            OnEventsCleared?.Invoke();
        }
    }
}