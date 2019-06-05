namespace SerialService.Services.Interfaces
{
    using DAL.Entities;

    public interface IVideoMaterialViewsByUsersService : IBaseService<VideoMaterialViewsByUsers>
    {
        bool UpdateEntity(VideoMaterialViewsByUsers entity);
    }
}