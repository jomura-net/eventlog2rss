using System;
using System.Configuration;
using Rss;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace EventLog2Rss
{
    public static class FeedUtil
    {
        /// <summary>
        /// RSS�𐶐��E�擾����B
        /// </summary>
        /// <param name="log">�C�x���g���O�I�u�W�F�N�g</param>
        /// <returns>RSS Feed</returns>
        internal static RssFeed GetFeed(EventLog log, Uri url, string type)
        {
            // �G���g���擾�ő吔������
            string maxLogEntryCountStr = ConfigurationManager.AppSettings["MaxLogEntryCount"];
            int maxLogEntryCount = 100;
            int count = maxLogEntryCount;
            if (!string.IsNullOrEmpty(maxLogEntryCountStr)
                && int.TryParse(maxLogEntryCountStr, out count))
            {
                maxLogEntryCount = count;
            }

            //�C�x���g���O�p�̐V�K�t�B�[�h�쐬
            RssChannel channel = new RssChannel();
            channel.Title = "EventLog(" + log.Log + ")";
            channel.Description = log.LogDisplayName;
            channel.Link = url;

            //�ΏۃG���g����V�������̂���S�Ē��o
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
                item.Author = string.IsNullOrEmpty(entry.UserName) ? "N/A" : entry.UserName;
                item.Description = entry.Message;

                string query = entry.Source + " " + entry.Message;
                if (query.Length > 127)
                {
                    query = query.Substring(0, 127);
                }
                query = Uri.EscapeUriString(query);
                item.Link = new Uri("http://www.google.co.jp/search?q=" + query);

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

#if false
                RssCategory InstanceId = new RssCategory();
                InstanceId.Domain = "InstanceId";
                InstanceId.Name = entry.InstanceId.ToString();
                item.Categories.Add(InstanceId);
#endif

                channel.Items.Add(item);
                //itemList.Add(item);
                
                //�w��ő吔�ɒB������A���o��������
                if (++count >= maxLogEntryCount)
                {
                    break;
                }
            }
            
            /*
            //RSS�I�u�W�F�N�g�ɋt���Œǉ�
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
            //�C�x���g���O�p�̐V�K�t�B�[�h�쐬
            RssFeed feed = new RssFeed(Encoding.UTF8);
            RssChannel channel = new RssChannel();
            channel.Title = "EventLogs";
            channel.Description = "?logname=Application|System|...<br />&type=Warning|Error";
            channel.Link = uri;

            //���[�J���R���s���[�^��̃C�x���g���O�̔z����擾����
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
