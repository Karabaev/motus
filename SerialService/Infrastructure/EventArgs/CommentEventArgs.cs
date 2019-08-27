namespace SerialService.Infrastructure.EventArgs
{
    using DAL.Entities;

    public class CommentEventArgs : IEventArgs<Comment>
    {
        public CommentEventArgs(Comment sender)
        {
            this.Sender = sender;
        }

        public Comment Sender { get; private set; }
    }
}