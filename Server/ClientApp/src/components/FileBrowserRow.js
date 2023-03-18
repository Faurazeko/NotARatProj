import React, { Component } from 'react';

export class FileBrowserRow extends Component {

	constructor(props) {
		super(props);
	}

	render() {

		return (
			<tr className="table-row" onClick={() => { this.props.click(this.props.data.FullPath) } }>
				<td>{this.props.data.Filename}</td>
				<td>{this.props.data.Size}</td>
				<td>{this.props.data.LocalDateModified}</td>
				<td>{this.props.data.LocalDateAccessed}</td>
				<td>{this.props.data.LocalDateCreated}</td>
				<td>{this.props.data.Type == 0 ? "File" : "Directory"}</td>
			</tr>
		);
	}

}