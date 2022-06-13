using System;
using System.Collections.Generic;
using UnityEngine;

namespace Analytics.Scripts.Analytics
{
    /// <summary>
    /// Using a pool of events and reuse them
    /// </summary>
    [Serializable]
    public class AnalyticsEvent
    {
        private static int INITIAL_POOL_SIZE = 1;

        private static readonly Queue<AnalyticsEvent> _availableAnalyticsEvents = new Queue<AnalyticsEvent>(INITIAL_POOL_SIZE);

        public static AnalyticsEvent GetAnalyticsEvent(long timestamp, string type, string data)
        {
            var analyticsEvent = GetNextAnalyticsEvent();

            analyticsEvent.timestamp = timestamp;
            analyticsEvent.type = type;
            analyticsEvent.data = data;

            return analyticsEvent;
        }

        public static AnalyticsEvent Deserialize(string analyticsEventStr)
        {
            var analyticsEvent = GetNextAnalyticsEvent();

            JsonUtility.FromJsonOverwrite(analyticsEventStr, analyticsEvent);

            return analyticsEvent;
        }

        public static string Serialize(AnalyticsEvent analyticsEvent)
        {
            return JsonUtility.ToJson(analyticsEvent);
        }

        public static void DisposeAnalyticsEvent(AnalyticsEvent analyticsEvent)
        {
            _availableAnalyticsEvents.Enqueue(analyticsEvent);
        }

        private static AnalyticsEvent GetNextAnalyticsEvent()
        {
            return _availableAnalyticsEvents.Count > 0 ? _availableAnalyticsEvents.Dequeue() : new AnalyticsEvent();
        }

        [NonSerialized]
        public long timestamp;

        public string type;
        public string data;

        // Should NOT be creating instances from outside
        private AnalyticsEvent()
        {
        }
    }
}