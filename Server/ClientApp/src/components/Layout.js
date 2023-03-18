import React, { Component } from 'react';
import { TopBar } from './TopBar';
import { NotificationPanel } from './NotificationPanel';
import { DialoguePanel } from './DialoguePanel';

export class Layout extends Component {
  static displayName = Layout.name;

  render() {
	  return (
		  <div className="one-hundred-percent">
			  <TopBar />
			  <NotificationPanel />
			  <DialoguePanel />

			  <div className="main-content">
				  {this.props.children}
			  </div>

		  </div>
	);
  }
}
