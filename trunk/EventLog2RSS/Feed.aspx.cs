using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Diagnostics;
using Rss;
using System.Text;
using System.Collections.Generic;

namespace EventLog2Rss
{
    public partial class Feed : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //イベントログ種別をクエリ文字列から取得する。
            string logname = Request.QueryString["logname"];

            //イベントログを取得する。
            EventLog[] eventLogs = EventLogUtil.GetEventLog(logname);
            RssFeed feed;
            if (eventLogs.Length == 1)
            {
                //表示する分類レベル
                string type = Request.QueryString["type"];
                //イベントログが1種別のみの場合、その種別のRSSを生成・取得する。
                feed = FeedUtil.GetFeed(eventLogs[0], MakeUrl(), type);
            }
            else
            {
                //QueryStringを除外する。
                UriBuilder urlb = new UriBuilder(MakeUrl());
                urlb.Query = null;

                //イベントログが1種別に特定できなかった場合、種別選択用のRSSを生成・取得する。
                feed = FeedUtil.GetSelectFeed(eventLogs, urlb.Uri);
            }

            //RSSを出力する。
            Response.ContentType = "text/xml";
            feed.Write(Response.OutputStream);
            Response.End();
        }

        string host = ConfigurationManager.AppSettings["Host"];
        string portStr = ConfigurationManager.AppSettings["Port"];

        /// <summary>
        /// アプリケーション構成ファイルでホスト名、ポート番号が指定されている場合、
        /// URL中のホスト名、ポート番号を指定された値で置換する。
        /// </summary>
        /// <returns>置換された後のURL</returns>
        Uri MakeUrl()
        {
            UriBuilder urib = new UriBuilder(Request.Url);
            if (!string.IsNullOrEmpty(host))
            {
                urib.Host = host;
            }
            int port = 80;
            if (!string.IsNullOrEmpty(portStr) && int.TryParse(portStr, out port))
            {
                urib.Port = port;
            }
            return urib.Uri;
        }

    }//eof class
}//eof namespace
