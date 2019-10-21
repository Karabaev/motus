namespace Shared.EntityActions
{
    using Model;

    public class CommentEntityActionsArgs
    {
        public CommentEntityActionsArgs(CommentEntityActionsModel sender, EntityActionTypes type)
        {
            this.Sender = sender;
            this.Type = type;
        }

        public CommentEntityActionsModel Sender { get; private set; }
        public EntityActionTypes Type { get; private set; }
    }
}