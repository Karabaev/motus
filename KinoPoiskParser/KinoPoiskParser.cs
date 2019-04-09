namespace KinoPoiskParser
{
    using AngleSharp.Parser.Html;
    using AngleSharp.Dom.Html;
    using System.Net;
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Парсер КиноПоиска по адресу страницы.
    /// </summary>
    public class KPParser
    {
        /// <param name="absolutePath">АБСОЛЮТНЫЙ! адрес страницы</param>
        public KPParser(string absolutePath)
        {
            Uri someUrl;
            string randomProxyUrl;
            Regex regex = new Regex("[0-9]+(?:\\.[0-9]+){3}:[0-9]+");///Регулярка для поиска ip+port
            //Получаем булево значение при попытке создания uri
            isCorrect = Uri.TryCreate(absolutePath, UriKind.Absolute, out someUrl);
            if (!isCorrect)
                throw new InvalidOperationException("URL-адрес некорректен.Убедитесь, что передается абсолютный адрес.");
            try
            {
                HttpWebRequest request = WebRequest.Create("https://raw.githubusercontent.com/clarketm/proxy-list/master/proxy-list.txt") as HttpWebRequest;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                WebHeaderCollection header = response.Headers;
                var encoding = ASCIIEncoding.ASCII;
                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                {
                    var onlyEnabeled = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (line.Contains('+') && line.Contains("RU"))
                        {
                            onlyEnabeled.Add(line);
                        }
                    }
                    string responseText = String.Join(String.Empty, onlyEnabeled.ToArray());
                    var someArr = regex.Matches(responseText);
                    randomProxyUrl = someArr[new Random().Next(someArr.Count - 1)].ToString();
                }
            }
            catch
            {
                throw new Exception("Ошибка при попытке запроса к списку прокси-серверов");
            }

            //Качаем нужную страницу в строковм виде
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Proxy = new WebProxy(randomProxyUrl);
                    htmlCode = client.DownloadString(someUrl);
                }
                catch
                {
                    throw new Exception("Ошибка при попытке создания прокси-соединения. Попробуйте еще раз.");
                }
            }
            DOM = new HtmlParser().Parse(htmlCode);
        }

        bool isCorrect;
        string htmlCode;
        /// <summary>
        /// Документ парсера, представляет виртуальный DOM
        /// </summary>
        IHtmlDocument DOM;

        /// <summary>
        /// Получить все Id на странице
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllId(string selector)
        {
            List<string> result = new List<string>();
            var elements = DOM.All.Where(a => a.ClassName == selector);

            foreach (var e in elements)
            {
                string attValue = e.Attributes.FirstOrDefault(a => a.Name == "id").Value;
                string id = Regex.Match(attValue, @"\d+").Value;
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }
                result.Add(id);
            }

            return result;
        }
    }
}
