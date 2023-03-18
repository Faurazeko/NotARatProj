using System.Net;
using System.Net.Sockets;
using NarLib;

namespace ClientWF
{
	class NetworkClient
	{
		static private IPAddress _serverIp = IPAddress.Parse(ConfigManager.Config.ServerIp);
		static private int _serverPort = ConfigManager.Config.ServerTcpPort;
		static private IPEndPoint _serverEndPoint = new IPEndPoint(_serverIp, _serverPort);

		private TcpClient _tcpClient = new TcpClient();
		private NetworkStream _tcpStream;

		private bool _isConnected = false;

		public bool IsListening { get; private set; }
		private Thread _listenThread;

		public delegate void RequestReceived(ControlRequestDto request);
		public RequestReceived Received;


		public RegStatus Connect()
		{
			if(!_isConnected)
			{
				_listenThread = new Thread(() => ListenLoop());

				try
				{
					_tcpClient.Connect(_serverEndPoint);

					_tcpStream = _tcpClient.GetStream();
				}
				catch
				{
					return RegStatus.TcpError;
				}

				_isConnected = true;
			}


			var regStatus = Register();

			switch (regStatus)
			{
				case TcpRegStatus.Ok:
					_listenThread.Start();
					return RegStatus.Ok;
				case TcpRegStatus.WrongCredentails:
					return RegStatus.WrongCredentails;
				case TcpRegStatus.ErrorRegEnd:
				case TcpRegStatus.UncategorizedError:
				default:
					return RegStatus.Error;

			}
		}

		private TcpRegStatus Register()
		{
			var credentials = new ClientConnect() { ClientId = ConfigManager.Config.ClientId, ClientSecret = ConfigManager.Config.ClientSecret };

			_tcpStream.Flush();
			_tcpStream.Write(Util.ToByteArray(credentials, true));


			var response = ReadFromStream<TcpRegResponse>();

			if(response == null)
				return TcpRegStatus.UncategorizedError;

			return response.Status;
		}

		private void ListenLoop()
		{
			IsListening = true;
			while (_isConnected)
			{
				var request = ReadFromStream<ControlRequestDto>();

				if (request == null)
					continue;

				Received?.Invoke(request);

			}
			IsListening = false;
		}

		private T ReadFromStream<T>()
		{
			try
			{
				while (!_tcpStream.DataAvailable)
					Thread.Sleep(100);

				var dataSizeBytes = new byte[4];
				_tcpStream.Read(dataSizeBytes, 0, 4);
				var dataSize = BitConverter.ToInt32(dataSizeBytes);
				var leftToReceive = dataSize;

				var bytes = new List<byte>();

				while (leftToReceive > 0)
				{
					int bytesToRead;
					var available = _tcpClient.Available;

					if (available > leftToReceive)
						bytesToRead = leftToReceive;
					else
						bytesToRead = available;

					var buff = new byte[bytesToRead];
					_tcpStream.Read(buff, 0, bytesToRead);

					bytes.AddRange(buff);

					leftToReceive -= bytesToRead;

					if (leftToReceive > 0)
						Thread.Sleep(100);
				}

				return Util.FromByteArray<T>(bytes.ToArray());
			}
			catch
			{
				return default(T);
			}
		}
	}

	public enum RegStatus
	{
		Ok = 0,
		Error,
		WrongCredentails,
		TcpError
	}
}
