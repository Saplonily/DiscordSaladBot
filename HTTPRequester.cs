using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

public class HTTPRequester
{
    /// <summary>  
    /// Http (GET/POST)  
    /// </summary>  
    /// <param name="url">请求URL</param>  
    /// <param name="parameters">请求参数</param>  
    /// <param name="method">请求方法</param>  
    /// <returns>响应内容</returns>  
    internal static string SendPost(string url, IDictionary<string, string> parameters, string method)
    {
        if (method.ToLower() == "post")
        {
            HttpWebRequest req = null;
            HttpWebResponse rsp = null;
            System.IO.Stream reqStream = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = method;
                req.KeepAlive = false;
                req.ProtocolVersion = HttpVersion.Version10;
                req.Timeout = 5000;
                req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));
                reqStream = req.GetRequestStream();
                reqStream.Write(postData, 0, postData.Length);
                rsp = (HttpWebResponse)req.GetResponse();
                Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                return GetResponseAsString(rsp, encoding);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (reqStream != null) reqStream.Close();
                if (rsp != null) rsp.Close();
            }
        }
        else
        {
            //创建请求  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?" + BuildQuery(parameters, "utf8"));

            //GET请求  
            request.Method = "GET";
            request.ReadWriteTimeout = 5000;
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)" +
            "Chrome/103.0.5060.134 Safari/537.36";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));

            //返回内容  
            string retString = myStreamReader.ReadToEnd();
            return retString;
        }
    }


    /// <summary>  
    /// 组装普通文本请求参数。  
    /// </summary>  
    /// <param name="parameters">Key-Value形式请求参数字典</param>  
    /// <returns>URL编码后的请求数据</returns>  
    internal static string BuildQuery(IDictionary<string, string> parameters, string encode)
    {
        StringBuilder postData = new StringBuilder();
        bool hasParam = false;
        IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
        while (dem.MoveNext())
        {
            string name = dem.Current.Key;
            string value = dem.Current.Value;
            // 忽略参数名或参数值为空的参数  
            if (!string.IsNullOrEmpty(name))
            {
                if (hasParam)
                {
                    postData.Append("&");
                }
                postData.Append(name);
                postData.Append("=");
                if (encode == "gb2312")
                {
                    postData.Append(HttpUtility.UrlEncode(value, Encoding.GetEncoding("gb2312")));
                }
                else if (encode == "utf8")
                {
                    postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                }
                else
                {
                    postData.Append(value);
                }
                hasParam = true;
            }
        }
        return postData.ToString();
    }

    /// <summary>  
    /// 把响应流转换为文本。  
    /// </summary>  
    /// <param name="rsp">响应流对象</param>  
    /// <param name="encoding">编码方式</param>  
    /// <returns>响应文本</returns>  
    internal static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
    {
        System.IO.Stream stream = null;
        StreamReader reader = null;
        try
        {
            // 以字符流的方式读取HTTP响应  
            stream = rsp.GetResponseStream();
            reader = new StreamReader(stream, encoding);
            return reader.ReadToEnd();
        }
        finally
        {
            // 释放资源  
            if (reader != null) reader.Close();
            if (stream != null) stream.Close();
            if (rsp != null) rsp.Close();
        }
    }
}