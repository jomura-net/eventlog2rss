using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace EventLog2Rss
{
    public static class EventLogUtil
    {
        internal static EventLog[] GetEventLog(string logname)
        {
            //ローカルコンピュータ上のイベントログの配列を取得する
            EventLog[] logs = EventLog.GetEventLogs();
            List<EventLog> loglist = new List<EventLog>(logs.Length - 1);
            foreach (EventLog log in logs)
            {
                if (log.Log == logname && "Security" != log.Log)
                {
                    return new EventLog[] { log };
                }

                if ("Security" != log.Log)
                {
                    loglist.Add(log);
                }
            }
            return loglist.ToArray();
        }

        internal static bool IsVisible(EventLogEntry entry, string type)
        {
            if (string.IsNullOrEmpty(type) || !Enum.IsDefined(typeof(EventLogEntryType), type))
            {
                return true;
            }
            if (0 == entry.EntryType) return false;

            EventLogEntryType visibleEntryType = (EventLogEntryType)Enum.Parse(typeof(EventLogEntryType), type);
            return visibleEntryType >= entry.EntryType;
        }
    }
}
