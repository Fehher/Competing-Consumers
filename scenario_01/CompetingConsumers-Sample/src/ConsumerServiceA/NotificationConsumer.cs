using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using ServiceMessage;

namespace ConsumerServiceA
{
    public class NotificationConsumer : IConsumer<INotification>
    {
        public async Task Consume(ConsumeContext<INotification> context)
        {
            var @event = context.Message;

            Console.ForegroundColor = ConsoleColor.Yellow;

            await Console.Out.WriteAsync("Consumer 01 \n");

            await Console.Out.WriteLineAsync($"ThreadI: {@event.ThreadId}, Name: {@event.Name}, CreatedAt: {@event.CreatedAt}");

            Thread.Sleep(context.Message.ThreadId * 200);

            await Task.FromResult(context);
        }
    }
}
