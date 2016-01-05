using HtmlSharp;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace QueryBaike
{
    public class BaiduBaike
    {
        public async static Task<string> QueryByKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return "";

            string keyWord = System.Net.WebUtility.UrlEncode(keyword);

            string url = await GetHttpResponse($"http://baike.baidu.com/search/word?word={keyWord}", true);

            if (url.Contains("没有找到") || url.Contains("search/none"))
            {
                return $"抱歉，没有找到与{keyword}相关的百科结果。";
            }

            // replace to wap baike to shorter the html
            url = url.Replace("baike", "wapbaike");

            if (!string.IsNullOrWhiteSpace(url))
            {
                string content = await GetHttpResponse(url);

                HtmlParser parser = new HtmlParser();
                Document detailDoc = parser.Parse(content);

                var para = detailDoc.Find<HtmlSharp.Elements.Tags.P>(".summary+p");
                
                if(para == null)
                    para = detailDoc.Find<HtmlSharp.Elements.Tags.P>(".summary>p");
                    
                if (para != null)
                {
                    string strText = System.Text.RegularExpressions.Regex.Replace(para.ToString(), "<[^>]+>", "");
                    strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");
                    strText = System.Text.RegularExpressions.Regex.Replace(strText, "\\[[^>]+\\]", "");
                    return strText;
                }
                else
                    return $"抱歉，没有找到与{keyword}相关的百科结果。";
            }
            else
                return $"抱歉，没有找到与{keyword}相关的百科结果。";
        }


        private async static Task<string> GetHttpResponse(string url, bool uriOnly = false)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);

            WebResponse response = await request.GetResponseAsync();

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
