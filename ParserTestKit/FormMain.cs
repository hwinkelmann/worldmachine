using Shared.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ParserTestKit
{
    public partial class FormMain : Form
    {
        Context context = new Context();
        Feed[] feeds;
        nJupiter.Web.Syndication.IFeedItem[] currentFeed;

        HtmlAgilityPack.HtmlDocument doc;

        public FormMain()
        {
            InitializeComponent();

            feeds = context.Feeds.ToArray();
            listBoxFeeds.Items.AddRange(feeds);
            //webBrowser.ScriptErrorsSuppressed = true;
        }

        private void listBoxFeeds_SelectedValueChanged(object sender, EventArgs e)
        {
            var feed = feeds[listBoxFeeds.SelectedIndex];

            using (WebClient client = new WebClient())
            {
                currentFeed = nJupiter.Web.Syndication.FeedReader.GetFeed(new Uri(feed.RssUrl)).Items.ToArray();

                listBoxItems.Items.Clear();
                listBoxItems.Items.AddRange(currentFeed.Select(d => d.Title).ToArray());
            }
        }

        private void listBoxItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = currentFeed[listBoxItems.SelectedIndex];

            //webBrowser.Url = item.Links.FirstOrDefault().Uri;
            textBox1.Text = item.Uri.ToString();
            buttonOpenInBrowser.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;

            doc = new HtmlAgilityPack.HtmlDocument();
            using (WebClient client = new WebClient())
                doc.LoadHtml(client.DownloadString(item.Uri.ToString()));

            button1_Click(null, null);
            button2_Click(null, null);
        }

        private void buttonOpenInBrowser_Click(object sender, EventArgs e)
        {
            var item = currentFeed[listBoxItems.SelectedIndex];
            Process.Start(item.Uri.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                labelError.Text = "";

                listViewTags.Items.Clear();
                var tagNodes = doc.DocumentNode.SelectNodes(textBoxTagXpath.Text);

                labelError.Text = DateTime.Now.ToString("s") + ": OK";

                if (tagNodes != null)
                    foreach (var tag in tagNodes.Where(t=>t.InnerText != null).Select(t => Crawler.Parsers.ParserBase.SanitizeText(t.InnerText)))
                        listViewTags.Items.Add(tag);

                if (tagNodes == null || tagNodes.Count == 0)
                    listViewTags.Items.Add("<empty list>");
            }
            catch (Exception exc)
            {
                labelError.Text = DateTime.Now.ToString("s") + ": " + exc.Message.Replace("\n", " - ").Replace("\r", "");
            }
        }

        private void textBoxTagXpath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                button1_Click(sender, e);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                label4.Text = "";

                textBoxText.Text = "";

                var tagNodes = doc.DocumentNode.SelectNodes(textBoxTextXPath.Text);

                label4.Text = DateTime.Now.ToString("s") + ": OK";


                if (tagNodes != null)
                    foreach (var tag in tagNodes.Where(t => t.InnerText != null).Select(t => Crawler.Parsers.ParserBase.SanitizeText(t.InnerText)))
                        textBoxText.Text += tag + "\n";

                if (tagNodes == null || tagNodes.Count == 0)
                    textBoxText.Text = "<empty list>";
            }
            catch (Exception exc)
            {
                label4.Text = DateTime.Now.ToString("s") + ": " + exc.Message.Replace("\n", " - ").Replace("\r", "");
            }
        }

        private void textBoxTextXPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                button2_Click(sender, e);
            }
        }
    }
}
