﻿using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;

namespace QueryBaike
{
    public class BaiduBaike
    {
        public class BaikeData
        {
            public string Summary { get; set; }
            public StorageFile Image { get; set; }
            public string Url { get; set; }
        }

        public async static Task<BaikeData> QueryByKeyword(string keyword)
        {
            BaikeData retData = new BaikeData();

            if (string.IsNullOrWhiteSpace(keyword))
                return retData;

            string keyWord = System.Net.WebUtility.UrlEncode(keyword);

            string searchResult = await GetHttpResponse($"http://baike.baidu.com/client/search?word={keyWord}");

            var jResult = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(searchResult);
            string id = jResult["lemmaId"]?.ToString();


            if (string.IsNullOrWhiteSpace(id))
            {
                retData.Summary = $"抱歉，没有找到与{keyword}相关的百科结果。";
                return retData;
            }
            
            string url = $"http://wapbaike.baidu.com/view/{id}.htm";

            //string clientUrl = $"http://baike.baidu.com/client/view/{id}.htm";

            retData.Url = url;
            if (!string.IsNullOrWhiteSpace(url))
            {
                string content = await GetHttpResponse(url);

                HtmlParser parser = new HtmlParser();

                var detailDoc = parser.Parse(content);

                var elements = detailDoc.QuerySelectorAll("div#main>div.card>p.summary~p");

                if (elements?.Length > 0)
                {
                    string strText = "";
                    elements.ToList().ForEach((e) =>
                    {
                        strText += e.TextContent;
                    });

                    strText = strText.Replace("\n", "").Replace("\n", "").Replace(" ", "");

                    if (strText.Length > 230)
                    {
                        strText = strText.Remove(230);
                        if (strText.Contains("。"))
                            strText = strText.Remove(strText.LastIndexOf("。"));
                    }

                    retData.Summary = strText + "\r\n转到小娜百科就可以查看详细的百科信息了哦！么么哒！";

                    var img = detailDoc.QuerySelector(".img-box>a>img");

                    string imageSource = img?.Attributes["src"].Value;

                    if (!string.IsNullOrWhiteSpace(imageSource))
                    {

                        var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync($"{keyword}.jpg", CreationCollisionOption.OpenIfExists);
                        using (HttpClient client = new HttpClient())
                        {
                            var bytes = await client.GetByteArrayAsync(imageSource);

                            await FileIO.WriteBytesAsync(file, bytes);
                        }

                        retData.Image = file;
                    }

                    return retData;
                }
                else
                {
                    retData.Summary = $"抱歉，没有找到与{keyword}相关的百科结果。";
                    return retData;
                }
            }
            else
            {
                retData.Summary = $"抱歉，没有找到与{keyword}相关的百科结果。";
                return retData;
            }
        }


        private async static Task<string> GetHttpResponse(string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);

            using (WebResponse response = await request.GetResponseAsync())
            {
                Stream s = response.GetResponseStream();

                using (StreamReader sr = new StreamReader(s))
                {
                    return await sr.ReadToEndAsync();
                }
            }
        }
    }
}
