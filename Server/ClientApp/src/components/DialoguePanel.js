import React, { Component } from 'react';
import { DialogueWindow } from "./DialogueWindow";
import "./Dialogue.css";

export class DialoguePanel extends Component {

	constructor(props) {
		super(props);

		this.show = this.show.bind(this);
		this.hide = this.hide.bind(this);

		window.dialoguePanel = this;

		this.state = { isShown: false, title: "Title", subTitle: "SubTitle", btns: [], renderTextField: false }
	}

	show(title, subTitle, btns, renderTextField) {

		if (title === undefined) {
			title = "Title";
		}

		if (subTitle === undefined) {
			subTitle = "";
		}

		if (btns === undefined) {
			btns = [{ text:"Close", callback: this.hide }];
		}


		this.setState({
			isShown: true,
			title: title,
			subTitle: subTitle,
			btns: btns,
			renderTextField: renderTextField
		});
	}

	hide(event) {
		if (event != undefined) {
			if (event.currentTarget !== event.target) {
				return;
			}
		}

		this.setState({ isShown: false });
	}

	render() {

		var className = `dialogue-panel ${this.state.isShown ? "" : "hide"}`;

		return (
			<div className={className} onClick={this.hide } >
				<DialogueWindow hideHandle={this.hide} title={this.state.title} subTitle={this.state.subTitle}
					btns={this.state.btns} renderTextField={this.state.renderTextField} />
			</div>
		);
	}

}