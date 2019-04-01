namespace SerialService.DAL.Entities
{
    public interface IMark : IBaseEntity
    {
        bool Value { get; set; }
        string UserIP { get; set; }
    }
}
