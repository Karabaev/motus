namespace SerialService.DAL.Entities
{
    /// <summary>
    /// Базовый класс сущности.
    /// </summary>
    public interface IBaseEntity
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        int ID { get; set; }

        bool Alike(IBaseEntity entity);
    }
}
