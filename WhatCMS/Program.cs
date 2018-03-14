using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WhatCMS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the full path to your text file of domain names. Example: C:\\Users\\bob\\Desktop\\domains.txt");
            Console.WriteLine("One domain per line, like this:");
            Console.WriteLine("domain1.com");
            Console.WriteLine("domain2.com");
            Console.WriteLine("Please do not close the program until you see: Done!");
            string userInput = Console.ReadLine();
            string[] listOfDomains = File.ReadAllLines(userInput);
            Console.WriteLine(listOfDomains.Length);
            var csv = new StringBuilder();
            var firstLine = string.Format("Domain,Default URL,Generator");
            csv.AppendLine(firstLine);

            Parallel.ForEach(listOfDomains, item =>
            {
                string domain = item;
                HtmlDocument page;
                string newLine;
                HttpWebRequest request;
                string responseUrl;
                Crawler fetchedUrl;
                try
                {
                    try
                    {
                        request = (HttpWebRequest)WebRequest.Create("http://" + domain);
                        request.Timeout = 10000;
                        request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
                        request.AllowAutoRedirect = true;
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        responseUrl = response.ResponseUri.ToString();
                        response.Close();
                        fetchedUrl = new Crawler(responseUrl);
                        newLine = string.Format("{0},{1},{2}", domain, responseUrl, fetchedUrl.Generator);
                    }
                    catch (Exception)
                    {
                        request = (HttpWebRequest)WebRequest.Create("http://www." + domain);
                        request.Timeout = 10000;
                        request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
                        request.AllowAutoRedirect = true;
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        responseUrl = response.ResponseUri.ToString();
                        response.Close();
                        fetchedUrl = new Crawler(responseUrl);
                        newLine = string.Format("{0},{1},{2}", domain, responseUrl, fetchedUrl.Generator);
                    }
                    csv.AppendLine(newLine);
                    Console.WriteLine("Checked " + domain);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error checking " + domain);
                    newLine = string.Format("{0},{1},{2}", domain, "Error", "Error");
                    csv.AppendLine(newLine);
                }
            });

            File.WriteAllText("WhatCMS-output.csv", csv.ToString());
            Console.WriteLine("Done!");
            Console.WriteLine("Open the file WhatCMS-output.csv in the directory the program file is running from.");
            Console.WriteLine("");
            Console.ReadLine();
        }
    }
}