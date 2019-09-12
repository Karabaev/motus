namespace SerialService.Infrastructure.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using DAL;
    using EventArgs;

    public class NotificationManager 
    {
        public NotificationManager(IAppUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.unitOfWork.Comments.OnCreate += this.Notification;
        }

        private void Notification(CommentEventArgs args)
        {

        }

        private readonly IAppUnitOfWork unitOfWork;
    }
}