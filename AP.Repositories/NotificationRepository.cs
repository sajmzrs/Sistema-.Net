using AP.Data;

namespace AP.Repositories
{
    public interface IRepositoryNotification : IRepositoryBase<Notification>
    {
    }

    public class RepositoryNotification : RepositoryBase<Notification>, IRepositoryNotification
    {
        public RepositoryNotification() : base()
        {
        }
    }
}
