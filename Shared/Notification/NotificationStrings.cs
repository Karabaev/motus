namespace Shared.Notification
{
    using System.Collections.Generic;

    public enum StringNames
    {
        Comment_created_caption_admin,
        Comment_changed_caption_admin,
        Comment_removed_caption_admin,
        Comment_created_text_admin,
        Comment_changed_text_admin,
        Comment_removed_text_admin
    }

    public static class NotificationStrings
    {
        static NotificationStrings()
        {
            Strings = new Dictionary<StringNames, string>
            {
                {  StringNames.Comment_created_caption_admin, "Новый комментарий" },
                {  StringNames.Comment_changed_caption_admin, "Комментарий изменен" },
                {  StringNames.Comment_removed_caption_admin, "Комментарий удален" },
                {  StringNames.Comment_created_text_admin, "Новый комментарий" },
                {  StringNames.Comment_changed_text_admin, "Комментарий изменен" },
                {  StringNames.Comment_removed_text_admin, "Комментарий удален" }
            };
        }

        public static string GetString(StringNames name)
        {
            return Strings[name];
        }

        private static Dictionary<StringNames, string> Strings { get; set; }
    }
}