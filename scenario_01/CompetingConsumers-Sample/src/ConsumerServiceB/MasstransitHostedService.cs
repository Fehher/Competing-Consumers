using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace ConsumerServiceB
{
    public class MasstransitHostedService : IHostedService
    {
        private readonly IBusControl _busControl;

        public MasstransitHostedService(IBusControl busControl) => _busControl = busControl;

        public async Task StartAsync(CancellationToken cancellationToken) =>
            await _busControl.StartAsync(cancellationToken);

        public async Task StopAsync(CancellationToken cancellationToken) =>
            await _busControl.StopAsync(cancellationToken);
    }
}
