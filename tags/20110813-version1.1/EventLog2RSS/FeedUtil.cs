using System;
using System.Configuration;
using Rss;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Web;

namespace EventLog2Rss
{
    public static class FeedUtil
    {
        /// <summary>
        /// RSSを生成・取得する。
        /// </summary>
        /// <param name="log">イベントログオブジェクト</param>
        /// <returns>RSS Feed</returns>
        internal static RssFeed GetFeed(EventLog log, Uri url, string type)
        {
            // エントリ取得最大数を決定
            string maxLogEntryCountStr = ConfigurationManager.AppSettings["MaxLogEntryCount"];
            int maxLogEntryCount = 100;
            int count = maxLogEntryCount;
            if (!string.IsNullOrEmpty(maxLogEntryCountStr)
                && int.TryParse(maxLogEntryCountStr, out count))
            {
                maxLogEntryCount = count;
            }

            //イベントログ用の新規フィード作成
            RssChannel channel = new RssChannel();
            channel.Title = "EventLog(" + log.Log + ")";
            channel.Description = log.LogDisplayName;
            channel.Link = url;

            //対象エントリを新しいものから全て抽出
            //List<RssItem> itemList = new List<RssItem>();
            count = 0;
            for (int i = log.Entries.Count - 1; i >= 0; i--)
            {
                EventLogEntry entry = log.Entries[i];

                if (!EventLogUtil.IsVisible(entry, type)) continue;

                RssItem item = new RssItem();
                item.Title = "[" + entry.EntryType.ToString()
                    + "] " + entry.Source;
                item.PubDate = entry.TimeGenerated.ToUniversalTime();
                if (!string.IsNullOrEmpty(entry.UserName))
                {
                    item.Author = entry.UserName;
                }
                item.Description = entry.Message;

                string query = entry.Source + " " + entry.Message;
                query = query.Replace('\n', ' ');
                if (query.Length > 127)
                {
                    query = query.Substring(0, 127);
                }
                //query = Uri.EscapeUriString(query); //SecurityCenter%20Windows%20%E3%82%BB%E3%82%AD%E3%83%A5%E3%83%AA%E3%83%86%E3%82%A3%20%E3%82%BB%E3%83%B3%E3%82%BF%E3%83%BC%20%E3%82%B5%E3%83%BC%E3%83%93%E3%82%B9%E3%82%92%E9%96%8B%E5%A7%8B%E3%81%97%E3%81%BE%E3%81%97%E3%81%9F%E3%80%82
                query = HttpUtility.UrlEncode(query); //SecurityCenter+Windows+%e3%82%bb%e3%82%ad%e3%83%a5%e3%83%aa%e3%83%86%e3%82%a3+%e3%82%bb%e3%83%b3%e3%82%bf%e3%83%bc+%e3%82%b5%e3%83%bc%e3%83%93%e3%82%b9%e3%82%92%e9%96%8b%e5%a7%8b%e3%81%97%e3%81%be%e3%81%97%e3%81%9f%e3%80%82
                Debug.WriteLine("query : " + query);
                item.Link = new Uri("http://www.google.co.jp/search?q=" + query);

                RssGuid guid = new RssGuid();
                guid.PermaLink = false;
                //とりあえず、発行時刻をglobally unique identifierに設定してみる
                guid.Name = entry.TimeGenerated.ToUniversalTime().ToString("yyyyMMdd_HHmmss.fff");
                item.Guid = guid;

                RssCategory EntryType = new RssCategory();
                EntryType.Domain = "EntryType";
                EntryType.Name = entry.EntryType.ToString();
                item.Categories.Add(EntryType);

                RssCategory Source = new RssCategory();
                Source.Domain = "Source";
                Source.Name = entry.Source;
                item.Categories.Add(Source);

                RssCategory Category = new RssCategory();
                Category.Domain = "Category";
                Category.Name = entry.Category;
                item.Categories.Add(Category);

                RssCategory InstanceId = new RssCategory();
                InstanceId.Domain = "InstanceId";
                InstanceId.Name = entry.InstanceId.ToString();
                item.Categories.Add(InstanceId);

                channel.Items.Add(item);
                //itemList.Add(item);
                
                //指定最大数に達したら、抽出処理完了
                if (++count >= maxLogEntryCount)
                {
                    break;
                }
            }
            
            /*
            //RSSオブジェクトに逆順で追加
            channel.Items.Capacity = itemList.Count;
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                channel.Items.Add(itemList[i]);
            }
            */

            AddEmptyItemToEmptyChannel(channel);
            RssFeed feed = new RssFeed(Encoding.UTF8);
            feed.Channels.Add(channel);
            return feed;
        }

        internal static RssFeed GetSelectFeed(EventLog[] logs, Uri uri)
        {
            //イベントログ用の新規フィード作成
            RssFeed feed = new RssFeed(Encoding.UTF8);
            RssChannel channel = new RssChannel();
            channel.Title = "EventLogs";
            channel.Description = "?logname=Application|System|...<br />&type=Warning|Error";
            channel.Link = uri;

            //ローカルコンピュータ上のイベントログの配列を取得する
            foreach (EventLog log in logs)
            {
                RssItem item = new RssItem();
                item.Title = log.LogDisplayName;
                item.Link = new Uri(uri + "?logname=" + log.Log);
                channel.Items.Add(item);
            }

            AddEmptyItemToEmptyChannel(channel);
            feed.Channels.Add(channel);
            return feed;
        }

        internal static void AddEmptyItemToEmptyChannel(RssChannel channel)
        {
            if (channel.Items.Count == 0)
            {
                RssItem item = new RssItem();
                item.Title = "(no data)";
                channel.Items.Add(item);
            }
        }

    }//eof class
}//eof namespace
