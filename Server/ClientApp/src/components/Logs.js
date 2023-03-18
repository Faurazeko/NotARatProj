import React, { Component } from 'react';
import withRouter from "./withRouter";
import "./Logs.css";

class Logs extends Component {


	constructor(props) {

		super(props);
		this.state = { data: undefined, loading: true, showRequests: true, showConnections: true, showErrors: true };

		this.populateData = this.populateData.bind(this);
		this.renderLogs = this.renderLogs.bind(this);
		this.checkboxsOnChange = this.checkboxsOnChange.bind(this);
	}

	componentDidMount() {
		this.populateData();
	}

	async populateData() {
		const response = await fetch(`/api/logs`);
		const data = await response.json();
		this.setState({ data: data, loading: false });
	}

	checkboxsOnChange(e) {

		switch (e.target.name) {
			case "requests":
				this.state.showRequests = e.target.checked;
				break;
			case "connections":
				this.state.showConnections = e.target.checked;
				break;
			case "errors":
				this.state.showErrors = e.target.checked;
				break;
			default:
				return;
		}

		this.populateData();
	}

	stringifyRequestType(number) {
        switch (number) {
			case 0:
				return "Command Line";
			case 1:
				return "Internal Function";
			case 2:
			default:
				return "Unknown";
        }
	}

	stringifyRequestStatus(number) {
		switch (number) {
			case 0:
				return "Awaiting";
			case 1:
				return "Timed Out";
			case 2:
				return "Failed";
			case 3:
				return "Succeeded";
			default:
				return "Unknown";
		}
	}

	renderLogs() {

		console.log(this.state.data);

		return (
			<div className="container mt-3">
				<div className="object logs-header-box mt-2 p-2 px-3">

					<h1 className="text-center " style={{ width: "100%" }}>Logs Viewer</h1>

					<h4>What to display</h4>
					<div className="row-flex">
						<label className="my-form">
							<input type="checkbox" name="requests" defaultChecked onChange={this.checkboxsOnChange} />
							Requests
						</label>

						<label className="my-form">
							<input type="checkbox" name="connections" defaultChecked onChange={this.checkboxsOnChange} />
							Connections
						</label>

						<label className="my-form">
							<input type="checkbox" name="errors" defaultChecked onChange={this.checkboxsOnChange} />
							Errors
						</label>
					</div>

				</div>

				<div>
					{this.state.data.map((e, i) => {
						return(
							<div key={ `logElement${i}` } className="logs-element">

								<div className="logs-number">Request #{e.id}</div>

								<div className="log-line">Target:
									<a className="log-line-value" href={`/client/${e.clientId}`}>
										ID {e.clientId} [{e.client.hostname}]
									</a>
								</div>
								<div className="log-line">
									Request:
									<span className="log-line-value">{e.request}</span>
								</div>
								<div className="log-line">
									Created:
									<span className="log-line-value">
										{new Date(e.createdUtcDateTime).toLocaleString("en-us",
											{
												year: "numeric", month: "short", day: "2-digit",
												hour: "2-digit", minute: "2-digit", second: "2-digit",
												hourCycle: "h24"
											})}
									</span>
								</div>
								<div className="log-line">
									Type:
									<span className="log-line-value">{this.stringifyRequestType(e.type)}</span>
								</div>
								<div className="log-line">
									Status:
									<span className="log-line-value">{this.stringifyRequestStatus(e.status)}</span>
								</div>
								<div className="log-line">
									Result:
									<span className="log-line-value">
										{
											e.result === null ?
												<span className="disabled-text">[none]</span> :
												e.result
										}
									</span>
								</div>
								<div className="log-line">
									Files:
									<span className="log-line-value">
										{
											e.files.length <= 0 ?
												<span className="disabled-text">[none]</span> :
												e.files.map((e, i) => {
													return (
														<span key={i}><a href={ "/Clients/Files/" + e} >[file {i + 1}] </a></span>
													);
												})

										}
									</span>
								</div>

								<button>View</button>

							</div>
						);
					})}
				</div>

			</div>
		);
	}

	render() {
		var contents = this.state.loading ?
			<h1 className="text-center">Loading...</h1> : //make it better
			this.renderLogs();

		return contents;
	}

}

export default withRouter(Logs);