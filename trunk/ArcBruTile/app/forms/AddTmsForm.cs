﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Net;

namespace BruTileArcGIS
{
    public partial class AddTmsForm : Form
    {
        private TileMap selectedTileMap=null;
        private string providedServiceURL = string.Empty;
        private bool useServiceURL = true;

        public bool UseServiceURL
        {
            get { return useServiceURL; }
            set { useServiceURL = value; }
        }

        public string ProvidedServiceURL
        {
            get { return providedServiceURL; }
            set { providedServiceURL = value; }
        }
        private List<TileMap> tilemaps;

        public AddTmsForm()
        {
            InitializeComponent();
        }

        public TileMap SelectedTileMap
        {
            get { return selectedTileMap; }
            set { selectedTileMap = value; }
        }

        private void btnRetrieve_Click(object sender, EventArgs e)
        {
            string url = tbTmsUrl.Text + @"/" + cbbVersion.Text;
            WebClient client = new WebClient();
            // add useragent to request
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14");

            IWebProxy proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultCredentials;
            client.Proxy = proxy;

            byte[] theBytes = client.DownloadData(url);
            string test = Encoding.UTF8.GetString(theBytes);
            client.Dispose();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(test);


            XmlNodeList nodes=doc.GetElementsByTagName("TileMap");
            
            tilemaps=new List<TileMap>();
            foreach (XmlNode node in nodes)
            {
                TileMap tileMap=new TileMap();
                tileMap.Href = node.Attributes["href"].Value;
                tileMap.Srs = node.Attributes["srs"].Value;
                tileMap.Profile = node.Attributes["profile"].Value;
                tileMap.Title= node.Attributes["title"].Value;
                tilemaps.Add(tileMap);
            }

            lbServices.DataSource = tilemaps;
            lbServices.DisplayMember = "Title";
        }

        private void AddTmsForm_Load(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.ProvidedServiceURL = tbTmsUrl.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void lbServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbServices.SelectedItem != null)
            {
                selectedTileMap = (TileMap)lbServices.SelectedItem;
                btnOk.Enabled = true;
            }
        }

        private void checkBoxUseTMSURL_CheckedChanged(object sender, EventArgs e)
        {
            this.UseServiceURL = checkBoxUseTMSURL.Checked;
        }
    }
}
