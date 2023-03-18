import React, { Component } from 'react';
import { ClientMngBlock } from './ClientMngBlock';

export class Home extends Component {
	static displayName = Home.name;

	constructor(props) {
		super(props);
		this.state = { clients: [], loading: true };
	}

	componentDidMount()
	{
		this.populateClientsData();
	}

	async populateClientsData() {
		const response = await fetch("/api/client/management");
		const data = await response.json();
		this.setState({ clients: data, loading: false });
	}

	renderClientsList() {
		return (
			<div className="wrapper container-fluid">

				{this.state.clients.map((client, i) => {

					var keyValue = `client${client.id}`;

					return (<ClientMngBlock client={client} key={keyValue}></ClientMngBlock>);
				})}

			</div>
		);
	}

	render() {

		var contents = this.state.loading ?
			<div className="text-center">
				<h1 className="mt-3">Loading clients...</h1>

				<div className="spinner-border p-spinner" role="status">
					<span className="sr-only"></span>
				</div>
			</div> :
			this.renderClientsList();

		return contents;
	}

}
