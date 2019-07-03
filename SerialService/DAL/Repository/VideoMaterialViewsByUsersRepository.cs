//namespace SerialService.DAL.Repository
//{
//    using Entities;
//    using Context;
//    using Infrastructure.Core;
//    using Infrastructure.Core.Extensions;
//    using System.Linq;
//    using System.Data.Entity;

//    public class VideoMaterialViewsByUsersRepository : IVideoMaterialViewsByUsersRepository
//    {
//        public VideoMaterialViewsByUsersRepository(ApplicationDbContext context)
//        {
//            this.DB = context;
//        }

//        public EntityList<VideoMaterialViewsByUsers> GetAllEntities()
//        {
//            throw new System.NotImplementedException();
//           // return this.DB.VideoMaterialViewsByUsers.ToEntityList();
//        }

//        public VideoMaterialViewsByUsers GetEntity(int id)
//        {
//            throw new System.NotImplementedException();
//            //return this.DB.VideoMaterialViewsByUsers.FirstOrDefault(p => p.ID == id);
//        }

//        public bool AddEntity(VideoMaterialViewsByUsers entity)
//        {
//            throw new System.NotImplementedException();
//            //if (entity == null)
//            //    return false;

//            //this.DB.VideoMaterialViewsByUsers.Add(entity);
//            //return this.SaveChanges();
//        }
//        /// <summary>
//        /// Обновить материал
//        /// </summary>
//        public bool UpdateEntity(VideoMaterialViewsByUsers entity)
//        {
//            throw new System.NotImplementedException();
//            //if (entity == null)
//            //    return false;

//            //VideoMaterialViewsByUsers cache = this.GetEntity(entity.ID);

//            //if (cache == null)
//            //    return false;
//            //foreach (var item in entity.GetType().GetProperties())
//            //{
//            //    if (item.Name != "ID")
//            //        item.SetValue(cache, item.GetValue(entity));
//            //}
//            //return this.SaveChanges();
//        }

//        public bool RemoveEntity(int id)
//        {
//            throw new System.NotImplementedException();
//            //this.DB.VideoMaterialViewsByUsers.Remove(this.DB.VideoMaterialViewsByUsers.FirstOrDefault(e => e.ID == id));
//            //return this.SaveChanges();
//        }

//        public bool RemoveEntity(VideoMaterialViewsByUsers enity)
//        {
//            throw new System.NotImplementedException();
//            //this.DB.VideoMaterialViewsByUsers.Remove(enity);
//            //return this.SaveChanges();
//        }

//        public bool SaveChanges()
//        {
//            throw new System.NotImplementedException();
//            //return this.DB.SaveChanges() > 0;
//        }

//        private readonly ApplicationDbContext DB;
//    }
//}
