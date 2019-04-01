namespace SerialService.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using DAL.Entities;
    using Infrastructure.Core;
    using SerialService.Models;

    public interface IVideoMaterialService : IBaseService<VideoMaterial>
    {
        IEnumerable<ElasticVideoMaterial> ElasticGetAll();

        IEnumerable<VideoMaterial> GetByPartOfTitle(string partOfTitle);

		bool Update(int oldID, VideoMaterial newVideoMaterial);

        VideoMaterial GetLoaded(int? id);

        VideoMaterial GetVisibleToUser(int? id);

        bool AddMark(int? id, bool isPositiveMark);

        bool InvertMark(int? id, bool fromPositive);

        bool MarkArchive(VideoMaterial entity);

        bool UnmarkArchive(VideoMaterial entity);

        bool AddSubscribedUser(int? videoMaterialID, ApplicationUser user);

		bool RemoveSubscribedUser(int? videoMaterialID, ApplicationUser user);

		bool EditMaterial(VideoMaterial entity);

        EntityList<VideoMaterial> GetAllVisibleToUser();

        EntityList<VideoMaterial> GetVisibleToUserWithCondition(Func<VideoMaterial, bool> predicate);

        bool IsUserSubscribed(int? id, string userID);
    }
}