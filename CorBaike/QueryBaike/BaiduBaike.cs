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
            //return $"抱歉，没有找到与{keyword}相关的百科结果。";
            string content = "";
            string keyWord = System.Net.WebUtility.UrlEncode(keyword);

            //content = await GetHttpResponse($"http://baike.baidu.com/search?word={keyWord}&pn=0&rn=0&enc=utf8");
            content = await GetHttpResponse($"http://baike.baidu.com/search/word?word={keyWord}",true);
            
            if (content.Contains("没有找到")||content.Contains("search/none"))
            {
                return $"抱歉，没有找到与{keyword}相关的百科结果。";
            }
            
            // replace to wap baike to shorter the html
            content = content.Replace("baike","wapbaike");
            
            //HtmlParser parser = new HtmlParser();
            //Document doc = parser.Parse(content);

            //var tag = doc.Find<HtmlSharp.Elements.Tags.A>("dl.search-list a.result-title");
            if (!string.IsNullOrWhiteSpace(content))
            {
                content = await GetHttpResponse(content);

                Document detailDoc = parser.Parse(content);

                var div = detailDoc.Find<HtmlSharp.Elements.Tags.P>(".summary+p");

                string strText = System.Text.RegularExpressions.Regex.Replace(div.ToString(), "<[^>]+>", "");
                strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");
                strText = System.Text.RegularExpressions.Regex.Replace(strText, "\\[[^>]+\\]", "");
                return strText;
            }
            else
                return $"抱歉，没有找到与{keyword}相关的百科结果。";
        }


        private async static Task<string> GetHttpResponse(string url,bool uriOnly=false)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);

            WebResponse response = await request.GetResponseAsync();
            
            if(uriOnly)
                return response.ResponseUri?.AbsoluteUri;
            
            Stream s = response.GetResponseStream();

            using (StreamReader sr = new StreamReader(s))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}
