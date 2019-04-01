namespace KinoPoiskParser
{
    using System.Linq;
    using AngleSharp.Parser.Html;
    using AngleSharp.Dom.Html;
    using System.Net;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

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
                    string responseText = reader.ReadToEnd();
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
        /// Возвращает объект модели страницы фильма,содержащей основные значния.
        /// </summary>
        /// <returns>Модель страницы фильма</returns>
        public KinopoiskItemModel Parse()
        {
            try
            {
                KinopoiskItemModel model = new KinopoiskItemModel
                {
                    Title = GetFirstByClass(Resource.TitleSelector).Split('<')[0].Trim(),
                    Duration = GetFirstByClass(Resource.DurationSelector).
                    Split(GetFirstByClass(Resource.DurationSelector).FirstOrDefault(s => char.IsSeparator(s) ||
                    char.IsPunctuation(s)))[0],//HACK:Отбитый наглухо костыль для получения продолжительности.Хардкод
                    OriginalTitle = GetAllByAttribute(Resource.OriginalTitleSelector)[0].Trim(),
                    FilmMakers = GetByTagOnAttribute(Resource.FilmMakersSelector, "a"),
                    Actors = GetByTagOnAttribute(Resource.ActorsSelector, "a").Where(a => (a.Any(c => c != '.'))).ToList(),//HACK:хардкод
                    Countries = GetAllByAttributeContains("a", "href", Resource.CountriesSelector),
                    Genres = GetAllByAttributeContains("a", "href", Resource.GenresSelector),
                    ReleaseDate = GetAllByAttributeContains("a", "href", Resource.YearSelector)[0].Trim(),
                    PoserHref = GetAttributeValue(Resource.PosterSelector, "src")[0],
                    KinopoiskID = GetAttributeValue(Resource.IDSelector, Resource.IDPropName)[0].Trim(),
                    IDMB = GetAllByAttribute("[style='color:#999;font:100 11px tahoma, verdana']")[0].Split(' ')[1].Trim()
                };
                return model;
            }
            catch
            {
                throw new Exception("Ошибка при попытке парсинга. Введен некорректный url-адресс.");
            }
        }

        /// <summary>
        /// Возвращает содержимое элемента в строковом типе. Поиск по классу.
        /// </summary>
        private string GetFirstByClass(string className)
        {
            string result = null;
            if (DOM != null)
            {
                var element = DOM.All.FirstOrDefault(m => m.ClassName == className);
                if (element!=null)
                {
                    result = element.InnerHtml;
                };
            }
            return result;
        }

        /// <summary>
        /// Возвращает содержимое элемента в строковом типе. Поиск по атрибуту с заданным значением.
        /// </summary>
        private List<string> GetAllByAttribute(string atr)
        {
            List<string> result = new List<string>();
            if (DOM != null)
            {
                var elements = DOM.QuerySelectorAll(atr).ToList();
                foreach(var el in elements)
                {
                    result.Add(el.InnerHtml.Trim());
                }
            }
            return result;
        }

        /// <summary>
        /// Возвращает содержимое элемента в строковом типе. Поиск по тегу, имеющему атрибут с заданным значением.
        /// </summary>
        /// <param name="tag">Название тэга</param>
        /// <param name="atr">Название атрибута</param>
        /// <param name="value">Значение или часть, входящая в него</param>
        private List<string> GetAllByAttributeContains(string tag,string atr,string value)
        {
            List<string> result = new List<string>();
            if (DOM != null)
            {
                var elements = DOM.QuerySelectorAll(tag).Where
                    (e=>e.Attributes.Any(a=>a.Name==atr&&a.Value.Contains(value)&&
                    e.InnerHtml.Length!=0));//HACK:Самое конченное в мире решение, но по другому никак
                foreach (var el in elements)
                {
                    result.Add(el.InnerHtml.Trim());
                }
            }
            return result;
        }

        /// <summary>
        /// Возвращает содержимое элемента в строковом типе. 
        /// Поиск обрабатывает элемент являющийся родительским, 
        /// и получает,помещенное в дочерний,значение 
        /// </summary>
        /// <param name="atr">Атрибут родительского элемента</param>
        /// <param name="tag">Тэг дочернего элемента</param>
        private List<string> GetByTagOnAttribute(string atr,string tag)
        {
            List<string> result = new List<string>();
            foreach (var item in GetAllByAttribute(atr))
            {
                var dom = new HtmlParser().Parse(item);
                var targetLine = dom.QuerySelector(tag).InnerHtml;
                result.Add(targetLine.Trim());
            }
            return result;
        }

        /// <summary>
        /// Возвращает значение атрибута элемента в строковом типе.
        /// </summary>
        /// <param name="selector">Любой селектор элемента</param>
        /// <param name="targetAtr">Название целевого атрибута</param>
        private List<string> GetAttributeValue(string selector,string targetAtr)
        {
            List<string> result = new List<string>();
            var elements = DOM.QuerySelectorAll(selector);
            foreach(var item in elements)
            {
                result.Add(item.Attributes.FirstOrDefault(a => a.Name == targetAtr).Value.Trim());
            }
            return result;
        }
    }
}
