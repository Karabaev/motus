namespace SerialService.Services.Interfaces
{
    using DAL.Entities;

    public interface IUserParamService : IBaseService<UserParam>
    {
        UserParam Get(string name, string userId);
    }
}