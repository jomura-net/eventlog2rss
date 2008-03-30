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
            EventLogEntryType entryType = (EventLogEntryType)Enum.Parse(typeof(EventLogEntryType), type);
            switch (entryType)
            {
                case EventLogEntryType.Error:
                    if (entry.EntryType == EventLogEntryType.Warning) return false;
                    goto case EventLogEntryType.Warning;
                case EventLogEntryType.Warning:
                    if (entry.EntryType == EventLogEntryType.Information) return false;
                    break;
            }
            return true;
        }
    }
}
