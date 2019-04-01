namespace SerialService.Services.Interfaces
{
    using DAL.Entities;

    public interface IVideoMarkService : IBaseService<VideoMark>
    {
        bool InverValue(VideoMark entity);
    }
}