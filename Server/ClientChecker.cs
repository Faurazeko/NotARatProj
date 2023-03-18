using Server.Data;

namespace Server
{
	public class ClientChecker : IHostedService
	{

		private readonly IServiceScopeFactory _serviceProvider;
		private Timer _checkingTimer;

		public ClientChecker(IServiceScopeFactory serviceScopeFactory) => _serviceProvider = serviceScopeFactory;

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_checkingTimer = new Timer(ExecuteCheckingTimer, null, 0, 600000); //10 min

			return Task.CompletedTask;
		}

		public void ExecuteCheckingTimer(object? state)
		{
			var scope = _serviceProvider.CreateScope();
			var clientRepo = scope.ServiceProvider.GetService<IClientRepo>();

			foreach (var item in clientRepo.GetAll())
			{
				if ( (DateTime.UtcNow - item.LastUpdateUtcTime) > TimeSpan.FromMinutes(6))
					item.IsOnline = false;
			}

			clientRepo.SaveChanges();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_checkingTimer.Dispose();

			return Task.CompletedTask;
		}
	}
}
