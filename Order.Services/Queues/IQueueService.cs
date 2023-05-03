using Order.Common.Models;

namespace Order.Services.Queues
{
    public interface IQueueService
    {
        void SendMessage(QueueEmailModel emailModel);
    }
}
