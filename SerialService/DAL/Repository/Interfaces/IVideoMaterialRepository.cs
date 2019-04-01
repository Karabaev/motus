namespace SerialService.DAL.Repository
{
    using Entities;

    public interface IVideoMaterialRepository : IRepository<VideoMaterial>
    {
        VideoMaterial GetEntityNotLazyLoad(int id);
    }
}


