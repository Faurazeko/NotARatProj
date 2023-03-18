import React, { Component } from 'react';
import withRouter from "./withRouter";
import "./ClientStream.css";

class ClientStream extends Component {

	constructor(props) {
		super(props);


		this.state = { client: undefined, loading: true, loadingString: "Loading client data..." };
		this.populateClientData = this.populateClientData.bind(this);
	}

	componentDidMount() {
		this.populateClientData();
	}

	async populateClientData() {
		const response = await fetch(`/api/client/management/${this.props.params.clientId}`);
		const data = await response.json();
		this.setState({ client: data, loading: false });
	}

	renderContent() {
		return (
			<div className="container mt-3 stream-container">
				<div className="main-window object me-1 p-2">
					<h5>
						Showing for
						<span className="disabled-text ms-2">
							[{this.state.client.id}] {this.state.client.hostname}
						</span>
					</h5>

					<div className="stream-window">
						{/*<img style={{ width: "100%", height: "100%" }} src="/api/client/stream/video" onLoad={(e) => { e.target.src = `/Clients/Files/kek.png?time=${new Date()}` } }>*/}
						{/*</img>*/}
						{/*<div className="stream-text">*/}
						{/*	<i class="bi bi-camera-video-off-fill text-p"></i>*/}
						{/*	<h1 className="m-0 p-0">No video streams are available at the moment</h1>*/}
						{/*</div>*/}
					</div>

				</div>
				<div className="side-window object ms-1 p-2">
					kek
				</div>
			</div>
		);
	}

	render() {

		var contents = this.state.loading ?
			<div className="text-center">
				<h1 className="mt-3">{ this.state.loadingString}</h1>

				<div className="spinner-border p-spinner" role="status">
					<span className="sr-only"></span>
				</div>
			</div> :
			this.renderContent();

		return contents;
	}
}

export default withRouter(ClientStream);
