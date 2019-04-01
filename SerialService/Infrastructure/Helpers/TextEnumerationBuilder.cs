namespace SerialService.Infrastructure.Helpers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Text;

    public static class TextEnumerationBuilder
    {
        /// <summary>
        /// Собирает контейнер строк в одну строку, в которой элементы контейнера разделены разделителем.
        /// </summary>
        /// <param name="strings">Контейнер строк.</param>
        /// <param name="separatorStr">Разделитель.</param>
        public static MvcHtmlString CreateEnumeration(this HtmlHelper html, IList<string> strings, string separatorStr = ", ")
        {
            return new MvcHtmlString(string.Join(separatorStr, strings));
        }

        public static MvcHtmlString CreateEnumeration(this HtmlHelper html, IList<MvcHtmlString> strings, string separatorStr = ", ")
        {
            return new MvcHtmlString(string.Join(separatorStr, strings));
        }
    }
}