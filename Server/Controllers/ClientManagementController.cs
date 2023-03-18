using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NarLib;
using Server.Data;
using Server.Dtos;
using Server.Models;

namespace Server.Controllers
{
	[Route("api/client/management")]
	[ApiController]
	public class ClientManagementController : ControllerBase
	{
		private readonly IClientRepo _clientRepo;
		private readonly IRequestRepo _requestRepo;
		private readonly IMapper _mapper;

		public ClientManagementController(IClientRepo clientRepo, IRequestRepo requestRepo, IMapper mapper)
		{
			_clientRepo = clientRepo;
			_requestRepo = requestRepo;
			_mapper = mapper;
		}


		[HttpGet]
		public IActionResult GetClients()
		{
			var clients = _clientRepo.GetAll();
			var dtos = _mapper.Map<List<ClientMngDto>>(clients);
			return Ok(dtos);
		}

		[HttpGet("{id}")]
		public IActionResult GetClient(int id)
		{
			var client = _clientRepo.GetAll().FirstOrDefault(e => e.Id == id);
			var dto = _mapper.Map<ClientMngDto>(client);

			return Ok(dto);
		}


		[HttpPost("{id}/control")]
		public IActionResult SendControlRequest(int id, [FromBody] ControlModel ctrlModel)
		{
			var request = new ControlRequest()
			{ Type = ctrlModel.Type, Request = ctrlModel.Command, ClientId = id };

			if (ctrlModel.Type == ControlRequestType.InternalFunction)
				ctrlModel.ForceOfflineSend = false;

			var client = _clientRepo.Get(request.ClientId);

			if (client == null)
				return BadRequest("No such client");

			var isConnected = Communicator.ClientConnected(request.ClientId);

			if (!isConnected && !ctrlModel.ForceOfflineSend)
				return BadRequest("Client with such ID is not connected by TCP.");

			var trimmedCmd = ctrlModel.Command.Trim();

			if (trimmedCmd.StartsWith("NoLogCmd"))
			{
				var splittedCmd = trimmedCmd.Split(' ');

				var isValidInt = int.TryParse(splittedCmd[1], out int cmdId);

				if (!isValidInt)
					return BadRequest("There is no NoLogCmd with such ID.");

				var isValidId = Utils.NoLogCmds.TryGetValue(cmdId, out string actualCmd);

				if(!isValidId)
					return BadRequest("There is no NoLogCmd with such ID.");

				request.Id = cmdId;
				request.Request = actualCmd  + " " + String.Join(" ", splittedCmd.Skip(2).ToArray());

			}
			else
			{
				_requestRepo.Add(request);
				_requestRepo.SaveChanges();
			}

			string response;

			if (!isConnected)
				response = "Request queued.";
			else
				response = "Sended";

			Communicator.SendToClient(_mapper.Map<ControlRequestDto>(request), request.ClientId);

			return Ok(response);
		}
	}
}
