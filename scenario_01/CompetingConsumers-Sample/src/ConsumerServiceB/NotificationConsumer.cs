using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using ServiceMessage;

namespace ConsumerServiceB
{
    public class NotificationConsumer : IConsumer<INotification>
    {
        public async Task Consume(ConsumeContext<INotification> context)
        {
            var @event = context.Message;

            Console.ForegroundColor = ConsoleColor.Magenta;

            await Console.Out.WriteAsync("Consumer 02 \n");

            await Console.Out.WriteLineAsync($"ThreadI: {@event.ThreadId}, Name: {@event.Name}, CreatedAt: {@event.CreatedAt}");

            Thread.Sleep(context.Message.ThreadId * 200);

            await Task.FromResult(context);
        }
    }
}
