using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace WebApplicationTest
{
    public class BooksCrawler
    {

        //static async Task Main(string[] args)
        //{
        //    string jsonData = await booksInfo("0010764130");
        //    Console.WriteLine(jsonData);
        //}

        public  async Task<string> booksInfo(string num)
        {
            string url = $"https://www.books.com.tw/products/{num}?sloc=main";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");

            string response = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);

            var bookInfo = new Dictionary<string, string>();
            var detailInfo = new Dictionary<string, string>();

            var imgTag = htmlDocument.DocumentNode.SelectSingleNode("//img[@class='cover']");
            if (imgTag != null)
            {
                bookInfo["image"] = imgTag.GetAttributeValue("src", "");
            }

            var titleTag = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='mod type02_p002 clearfix']");
            if (titleTag != null)
            {
                bookInfo["title"] = titleTag.InnerText.Trim();
            }

            var authorTag = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='type02_p003 clearfix']");
            if (authorTag != null)
            {
                string author = Regex.Replace(authorTag.InnerText, @"\s+", " ").Trim();
                author = author.Replace(" 新功能介紹", "").Replace("已追蹤作者： [ 修改 ] 確定 取消", "");
                bookInfo["author"] = author;
            }

            var priceTag = htmlDocument.DocumentNode.SelectSingleNode("//ul[@class='price']");
            if (priceTag != null)
            {
                bookInfo["price"] = priceTag.InnerText.Trim();
            }

            var authorInfoTag = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='mod_b type02_m057 clearfix']");
            if (authorInfoTag != null)
            {
                string authorInfo = Regex.Replace(authorInfoTag.InnerText, @"\s+", " ").Trim().Replace("看更多", "");
                bookInfo["OTHER_INFO"] = authorInfo;
            }

            var infoTags = htmlDocument.DocumentNode.SelectNodes("//div[@class='bd']");
            if (infoTags != null)
            {
                foreach (var infoTag in infoTags)
                {
                    var infoItems = infoTag.SelectNodes(".//li");
                    if (infoItems != null)
                    {
                        foreach (var item in infoItems)
                        {
                            string infoText = Regex.Replace(item.InnerText, @"\s+", "");
                            var keyValue = infoText.Split('：');
                            if (keyValue.Length == 2)
                            {
                                detailInfo[keyValue[0]] = keyValue[1];
                            }
                        }
                    }
                }
            }

            string isbn = "";
            if (detailInfo.ContainsKey("ISBN"))
            {
                isbn = detailInfo["ISBN"];
                detailInfo.Remove("ISBN");
            }

            var result = new
            {
                ISBN = isbn,
                book_info = bookInfo,
                detail_info = detailInfo
            };

            return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        }
    }

}

