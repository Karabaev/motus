﻿namespace SerialService.Services.Interfaces
{
    using DAL.Entities;

    public interface ICommentMarkService : IBaseService<CommentMark>
    {
        bool InvertValue(CommentMark entity);
    }
}
