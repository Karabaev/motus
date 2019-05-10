namespace Updater.Shared
{
    public interface IModule
    {
        int Index { get; set; }
        IModule ParentModule { get; set; }
        string Name { get; set; }
        void Launch();
        void Show();
    }
}
