using System;

namespace ServiceMessage
{
    public interface INotification
    {
        int ThreadId { get; set; }
        string Name { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
