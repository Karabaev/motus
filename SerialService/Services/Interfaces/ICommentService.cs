namespace SerialService.Services.Interfaces
{
    using System;
    using Infrastructure.Core;
    using DAL.Entities;

    public interface ICommentService : IBaseService<Comment>
    {
        bool MarkArchive(Comment entity);
        bool UnmarkArchive(Comment entity);
        EntityList<Comment> GetAllUnmarkedArchive();
        EntityList<Comment> GetUnmarkedArchiveWithCondition(Func<Comment, bool> predicate);
        Comment GetUnmarkedArchiveScalarWithCondition(Func<Comment, bool> predicate);
        bool AddVote(int? id, bool isPositiveMark);
        bool RemoveVote(int? id, bool isPositiveMark);
        bool InvertVote(int? id, bool fromPositive);
    }
}
