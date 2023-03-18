import React, { Component } from 'react';
import { Notification } from './Notification';
import "./Notifications.css"

export class NotificationPanel extends Component {

	constructor(props) {
		super(props);

		this.notify = this.notify.bind(this);
		this.handleChildUnmount = this.handleChildUnmount.bind(this);

		window.notificationPanel = this;

		this.notifications = [];
		this.notificationId = 0;
	}

	notify(text, type, url) {
		this.notificationId += 3;

		this.notifications[this.notificationId] = { text: text, type: type, url: url };

		this.forceUpdate();
	}

	handleChildUnmount(childId) {

		this.notifications.splice(childId, 1);

		this.forceUpdate();
	}

	render() {
		return (
			<div className="notification-panel">

				{this.notifications.map((notif, i) => {
					return (<Notification key={`notification${i}`}
						status={notif.type} text={notif.text} url={notif.url}
						unmountMe={this.handleChildUnmount} childId={i} />);
				})}
			</div>
		);
	}

}