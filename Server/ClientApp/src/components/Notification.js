import React, { Component } from 'react';

export class Notification extends Component {

	constructor(props) {
		super(props);

		this.hide = this.hide.bind(this);
		this.unmount = this.unmount.bind(this);

		var textLength = this.props.text === undefined ? 200 : this.props.text.length;

		var timeMsUntilFadeOut = ((textLength / 1000) * 1000 * 60) + 5000;
		var destructTimerId = setTimeout(this.hide, timeMsUntilFadeOut);

		var text = this.props.text === undefined ? "No text was provided for this notification :(" : this.props.text;

		if (this.props.url != undefined && this.props.url != "") {
			text += `\n<a href="${this.props.url}">View details</a>`
		}

		this.state = { timeMsUntilFadeOut: timeMsUntilFadeOut, destructTimerId: destructTimerId, text: text }
	}

	hide() {
		this.setState({ extraClass: "notification-hide" });

		setTimeout(this.unmount, 1000);
	}

	unmount() {
		this.props.unmountMe(this.props.childId);
	}

	render() {

		var extraClass = `notification ${this.state.extraClass === undefined ? "" : this.state.extraClass} `;
		var header = "Notification"

		switch (this.props.status.toLowerCase()) {
			case "suc":
				extraClass += "notification-success";
				break;
			case "wng":
				extraClass += "notification-warning";
				break;
			case "dng":
				extraClass += "notification-danger";
				break;
			case "inf":
			default:
				break;
		}

		return (
			<div className={extraClass}>

				<div className="notification-header"> {header}</div>

				<div dangerouslySetInnerHTML={{ __html: this.state.text }}></div>



				<div className="notification-close" onClick={this.hide}></div>
			</div>
		);
	}

}