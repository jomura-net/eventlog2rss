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
            //�C�x���g���O��ʂ��N�G�������񂩂�擾����B
            string logname = Request.QueryString["logname"];

            //�C�x���g���O���擾����B
            EventLog[] eventLogs = EventLogUtil.GetEventLog(logname);
            RssFeed feed;
            if (eventLogs.Length == 1)
            {
                //�\�����镪�ރ��x��
                string type = Request.QueryString["type"];
                //�C�x���g���O��1��ʂ݂̂̏ꍇ�A���̎�ʂ�RSS�𐶐��E�擾����B
                feed = FeedUtil.GetFeed(eventLogs[0], MakeUrl(), type);
            }
            else
            {
                //QueryString�����O����B
                UriBuilder urlb = new UriBuilder(MakeUrl());
                urlb.Query = null;

                //�C�x���g���O��1��ʂɓ���ł��Ȃ������ꍇ�A��ʑI��p��RSS�𐶐��E�擾����B
                feed = FeedUtil.GetSelectFeed(eventLogs, urlb.Uri);
            }

            //RSS���o�͂���B
            Response.ContentType = "text/xml";
            feed.Write(Response.OutputStream);
            Response.End();
        }

        string host = ConfigurationManager.AppSettings["Host"];
        string portStr = ConfigurationManager.AppSettings["Port"];

        /// <summary>
        /// �A�v���P�[�V�����\���t�@�C���Ńz�X�g���A�|�[�g�ԍ����w�肳��Ă���ꍇ�A
        /// URL���̃z�X�g���A�|�[�g�ԍ����w�肳�ꂽ�l�Œu������B
        /// </summary>
        /// <returns>�u�����ꂽ���URL</returns>
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
