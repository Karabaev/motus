namespace SerialService.DAL.Entities
{
    using Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Базовый класс для всех материалов.
    /// </summary>
    public interface IMaterialItem : IBaseItem
    {
        /// <summary>
        /// Заголовок/название.
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// Текст/описание.
        /// </summary>
        string Text { get; set; }
        /// <summary>
        /// Дата добавления.
        /// </summary>
        DateTime? AddDateTime { get; set; }
        /// <summary>
        /// Дата обновления.
        /// </summary>
        DateTime? UpdateDateTime { get; set; }
        /// <summary>
        /// Автор.
        /// </summary>
        ApplicationUser Author { get; set; }
        /// <summary>ы
        /// Идентификатор автора.
        /// </summary>
        string AuthorID { get; set; }

        List<Theme> Themes { get; set; }
    }
}
