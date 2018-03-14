using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace WhatCMS
{
    public class Crawler
    {
        public HtmlDocument Page { get; set; }
        public string BodyString { get; set; }
        public string ResponseUrl { get; set; }
        public string Generator { get; set; }
        public Crawler(string url)
        {
            HtmlWeb loadedFromWeb = new HtmlWeb()
            {
                PreRequest = request =>
                {
                    // Make any changes to the request object that will be used.
                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    return true;
                }
            };
            loadedFromWeb.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:5.0.1) Gecko/20100101 Firefox/5.0.1";
            this.Page = loadedFromWeb.Load(url);
            this.ResponseUrl = loadedFromWeb.ResponseUri.ToString();
            this.Generator = GetMetaGenerator(Page);

        }
        public static string GetMetaGenerator(HtmlDocument fetchedDocument)
        {
            try
            {
                var findMetaGenerator = fetchedDocument.DocumentNode.SelectNodes("//meta[translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='generator']").First();
                string metaGenerator;
                metaGenerator = findMetaGenerator.Attributes["content"].Value;
                metaGenerator = metaGenerator.ToLower();
                return metaGenerator;
            }
            catch (Exception)
            {
                string metaGenerator = "Not Found";
                return metaGenerator;
            }
        }
    }
}