namespace SerialService.Infrastructure.EventArgs
{
    using DAL.Entities;

    public interface IEventArgs<T> where T: IBaseEntity
    {
        T Sender { get; }
    }
}