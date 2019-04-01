namespace SerialService.DAL.Entities
{
    /// <summary>
    /// Базовый класс для оцениваемых сущностей.
    /// </summary>
    public interface IBaseItem : IBaseEntity
    {
        int PositiveMarkCount { get; set; }
        int NegativeMarkCount { get; set; }
    }
}
