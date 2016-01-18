using HtmlSharp;
using System;
using System.IO;
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

            string url = await GetHttpResponse($"http://baike.baidu.com/search/word?word={keyWord}", true);

            if (url.Contains("没有找到") || url.Contains("search/none"))
            {
                retData.Summary = $"抱歉，没有找到与{keyword}相关的百科结果。";
                return retData;
            }

            // replace to wap baike to shorter the html
            url = url.Replace("baike", "wapbaike");
            retData.Url = url;
            if (!string.IsNullOrWhiteSpace(url))
            {
                string content = await GetHttpResponse(url);

                HtmlParser parser = new HtmlParser();
                Document detailDoc = parser.Parse(content);

                var para = detailDoc.Find<HtmlSharp.Elements.Tags.P>(".summary+p");

                if (para == null)
                    para = detailDoc.Find<HtmlSharp.Elements.Tags.P>(".summary>p");

                if (para != null)
                {
                    string strText = System.Text.RegularExpressions.Regex.Replace(para.ToString(), "<[^>]+>", "");
                    strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");
                    strText = System.Text.RegularExpressions.Regex.Replace(strText, "\\[[^>]+\\]", "");
                    retData.Summary = strText;

                    var img = detailDoc.Find<HtmlSharp.Elements.Tags.Img>(".img-box>a>img");

                    if (!string.IsNullOrWhiteSpace(img?.Src))
                    {

                        var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync($"{keyword}.jpg", CreationCollisionOption.OpenIfExists);
                        using (HttpClient client = new HttpClient())
                        {
                            var bytes = await client.GetByteArrayAsync(img.Src);

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


        private async static Task<string> GetHttpResponse(string url, bool uriOnly = false)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);

            using (WebResponse response = await request.GetResponseAsync())
            {
                if (uriOnly)
                    return response.ResponseUri?.AbsoluteUri;

                Stream s = response.GetResponseStream();

                using (StreamReader sr = new StreamReader(s))
                {
                    return await sr.ReadToEndAsync();
                }
            }
        }
    }
}
