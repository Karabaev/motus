namespace SerialService.Infrastructure.EventArgs
{
    using Shared.Enums;
    using DAL.Entities;

    public class CommentEventArgs : IEventArgs<Comment>
    {
        public CommentEventArgs(Comment sender, EntityEventTypes type)
        {
            this.Sender = sender;
            this.Type = type;
        }

        public Comment Sender { get; private set; }
        public EntityEventTypes Type { get; private set; }
    }
}