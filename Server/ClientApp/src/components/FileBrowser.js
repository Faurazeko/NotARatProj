import React, { Component } from 'react';
import withRouter from "./withRouter";
import "./FileBrowser.css"
import { FileBrowserRow } from './FileBrowserRow';

class ClientSettings extends Component {


	constructor(props) {

		super(props);
		this.state = { client: undefined, dirContent: undefined, loading: true, headerToResize: undefined};

		this.populateClientData = this.populateClientData.bind(this);
		this.sendControl = this.sendControl.bind(this);
		this.renderContent = this.renderContent.bind(this);
		this.setNewDirData = this.setNewDirData.bind(this);
		this.headerOnClick = this.headerOnClick.bind(this);
		this.resizeHeader = this.resizeHeader.bind(this);
		this.stopHeaderResize = this.stopHeaderResize.bind(this);
		this.onRowClick = this.onRowClick.bind(this);
	}

	componentDidMount() {
		window.MySignalR.addEvent("DirData", this.setNewDirData);

		this.populateClientData();
	}

	setNewDirData(message) {

		var obj = JSON.parse(message);

		if (!obj.IsOk) {
			window.notificationPanel.notify(obj.ErrMsg, "dng", undefined);
			return;
		}


		this.setState({ dirContent: obj, loading: false });
	}

	async populateClientData() {
		const response = await fetch(`/api/client/management/${this.props.params.clientId}`)
			.then(response => {
				this.sendControl("NoLogCmd -1 C:/");

				return response.json();
			})
			.then(json => {
				this.setState({ client: json });
			});
	}

	async sendControl(command) {
		var data = { command: command, forceOfflineSend: false, type: 1 };

		await fetch(`/api/client/management/${this.props.params.clientId}/control`, {
			method: "POST",
			body: JSON.stringify(data),
			headers: {
				'Content-Type': 'application/json',
			},
		});
	}

	onRowClick(directory) {
		this.sendControl(`NoLogCmd -1 ${directory}`);
	}

	headerOnClick(e) {
		console.log("1");

		if (e.target !== e.currentTarget) return;

		this.setState({ headerToResize: e.currentTarget });

		document.onmousemove = this.resizeHeader;
		document.onmouseup = this.stopHeaderResize;

		console.log("kek");
	}

	resizeHeader(e) {

		console.log(2)

		var rect = this.state.headerToResize.getBoundingClientRect();

		console.log(this.state.headerToResize.style.width)
		console.log(`${e.clientX - rect.left}px`);

		this.state.headerToResize.style.width = `${e.clientX - rect.left}px`;
	}

	stopHeaderResize() {
		document.onmousemove -= this.resizeHeader;
		document.onmouseup -= this.stopHeaderResize;

		this.setState({ headerToResize: null });
	}

	renderContent() {
		return (
			<div className="container mt-3">
				<div className="window-main object me-1 p-2">
					<h5>
						Showing for
						<span className="disabled-text ms-2">
							[{this.state.client.id}] {this.state.client.hostname}
						</span>
					</h5>

					<div className="table-wrapper">
						<table>
							<thead>
								<tr className="table-row">
									<th onMouseDown={this.headerOnClick}>
										<div>Name</div>
									</th>

									<th onMouseDown={this.headerOnClick}>
										<div>Size</div>
									</th>

									<th onMouseDown={this.headerOnClick}>
										<div>Date Modified</div>
									</th>

									<th onMouseDown={this.headerOnClick}>
										<div>Date Accessed</div>
									</th>

									<th onMouseDown={this.headerOnClick}>
										<div>Date Created</div>
									</th>

									<th onMouseDown={this.headerOnClick}>
										<div>Type</div>
									</th>
								</tr>
							</thead>

							<tbody>

								{this.state.dirContent.Response.map((item, i) => {
									var key = `row-${i}`

									return <FileBrowserRow key={key} data={item} click={this.onRowClick} />;
								})}

							</tbody>
						</table>
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
			this.renderContent();

		return contents;
	}

}

export default withRouter(ClientSettings);