using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NarLib;
using Server.Data;
using Server.Models;
using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
	//todo: protection
	public class Communicator : IHostedService
	{
		private static int _port = 13370;

		private TcpListener _listener = new(IPAddress.Any, _port);
		private static Dictionary<int, TcpClient> _clients = new();
		private Timer? _listnerTimer = null;
		private Timer? _checkingTimer = null;

		private int _maxTimeoutCount = 10;
		private int _timeoutMs = 2000;

		private readonly IServiceScopeFactory _serviceProvider;

		public Communicator(IServiceScopeFactory serviceScopeFactory) => _serviceProvider = serviceScopeFactory;

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_listener.Start();

			_listnerTimer = new Timer(ExecuteListnerTimer, null, 0, 5000);
			_checkingTimer = new Timer(ExecuteCheckingTimer, null, 0, 600000); //10 min

			return Task.CompletedTask;
		}

		public void ExecuteListnerTimer(object? state)
		{
			var scope = _serviceProvider.CreateScope();
			var clientRepo = scope.ServiceProvider.GetService<IClientRepo>();

			TcpClient tcpClient = null!;
			Client dbClient = null!;

			if (!_listener.Pending())
				return;

			tcpClient = _listener.AcceptTcpClient();

			var regTry = 0;
			var netStream = tcpClient.GetStream();
			var isClientRegistred = false;


			Console.WriteLine($"--> TCP client connected. Remote EndPoint: {tcpClient.Client.RemoteEndPoint}");
			Console.WriteLine($"--> TCP: {tcpClient.Client.RemoteEndPoint} Waiting for registration.");
			Thread.Sleep(_timeoutMs);

			while (!isClientRegistred)
			{
				var sendResponse = true;
				var response = new TcpRegResponse();
				regTry++;

				try
				{
					Console.WriteLine($"--> TCP: {tcpClient.Client.RemoteEndPoint} Registration try {regTry}/{_maxTimeoutCount}");

					if (tcpClient.Available > 0)
					{
						var bytes = new byte[4];
						netStream.Read(bytes, 0, 4);
						var responseSize = BitConverter.ToInt32(bytes);

						var buff = new byte[responseSize];

						netStream.Read(buff, 0, responseSize);
						var clientConnect = Util.FromByteArray<ClientConnect>(buff.ToArray());

						dbClient = clientRepo!.Get(clientConnect.ClientId);

						if (dbClient == null)
						{
							Console.WriteLine($"--> TCP: {tcpClient.Client.RemoteEndPoint} No such client ID.");
							response.Status = TcpRegStatus.WrongCredentails;
						}
						else
						{
							if (dbClient.Secret == clientConnect.ClientSecret)
							{
								Console.WriteLine($"--> TCP: {tcpClient.Client.RemoteEndPoint} Registred as ClientID {dbClient.Id} [Hostname: {dbClient.Hostname}]");
								isClientRegistred = true;
								response.Status = TcpRegStatus.Ok;
							}
							else
							{
								Console.WriteLine($"--> TCP: {tcpClient.Client.RemoteEndPoint} Provided invalid credentials");
								response.Status = TcpRegStatus.WrongCredentails;
							}
						}

					}
					else
						sendResponse = false;
				}
				catch
				{
					Console.WriteLine($"--> TCP: {tcpClient.Client.RemoteEndPoint} Something went wrong while registering.");
					response.Status = TcpRegStatus.UncategorizedError;
				}

				if (isClientRegistred)
					Console.WriteLine($"--> TCP: {tcpClient.Client.RemoteEndPoint} Registred.");
				else
				{
					if (regTry >= _maxTimeoutCount)
					{
						Console.WriteLine($"--> TCP: {tcpClient.Client.RemoteEndPoint} Registration failed.");
						response.Status = TcpRegStatus.ErrorRegEnd;
						break;
					}

					Console.WriteLine($"--> TCP: {tcpClient.Client.RemoteEndPoint} Registration retry in {_timeoutMs} ms.");
					Thread.Sleep(_timeoutMs);
				}

				if(sendResponse)
					netStream.Write(Util.ToByteArray(response, true));

			}

			if (isClientRegistred && dbClient != null)
			{
				var msg = $"TCP client {tcpClient.Client.RemoteEndPoint} [ClientId = {dbClient.Id}] ";

				if (_clients.TryGetValue(dbClient.Id, out var _))
				{
					_clients.Remove(dbClient.Id);

					msg += "Reconnected.";
				}
				else
					msg += "Added, accepted and registred.";

				_clients.Add(dbClient.Id, tcpClient);
				Console.WriteLine(msg);

			}
			else
			{
				tcpClient.Close();
				tcpClient.Dispose();
			}
		}

		public static bool SendToClient(ControlRequestDto request, int clientId)
		{
			var exists = _clients.TryGetValue(clientId, out var client);

			if (!exists)
				return false;

			var netStream = client!.GetStream();

			try
			{
				netStream.Write(Util.ToByteArray(request, true));
			}
			catch
			{
				DisconnectClient(clientId); 
				return false;
			}

			return true;
		}

		public static bool ClientConnected(int clientId) => _clients.TryGetValue(clientId, out var _);

		public static void DisconnectClient(int clientId)
		{
			if (!_clients.TryGetValue(clientId, out var client))
				return;

			client!.Close();
			client!.Dispose();

			_clients.Remove(clientId);
		}

		private void ExecuteCheckingTimer(object? state)
		{
			var clientsToDelete = new List<int>();

			foreach (var item in _clients)
			{
				if (item.Value.Client.Poll(0, SelectMode.SelectRead))
				{
					byte[] buff = new byte[1];

					try
					{
						if (item.Value.Client.Receive(buff, SocketFlags.Peek) == 0)
							clientsToDelete.Add(item.Key);
					}
					catch
					{
						clientsToDelete.Add(item.Key);
					}

				}
			}

			if(clientsToDelete.Count() > 0)
			{

				var scope = _serviceProvider.CreateScope();
				var clientRepo = scope.ServiceProvider.GetService<IClientRepo>();

				foreach (var item in clientsToDelete)
				{
					DisconnectClient(item);

					var client = clientRepo.Get(item);

					if (client != null)
						client.IsOnline = false;

					Console.WriteLine($"TCP client {item} was disconnected by Communicator Checker.");
				}

				clientRepo.SaveChanges();
			}
		}


		public Task StopAsync(CancellationToken cancellationToken)
		{
			if(_listnerTimer != null)
				_listnerTimer.Dispose();

			_clients.Clear();
			_listener.Stop();

			return Task.CompletedTask;
		}
	}
}
