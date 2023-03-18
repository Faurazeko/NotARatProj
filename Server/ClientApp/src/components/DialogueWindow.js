import React, { Component } from 'react';

export class DialogueWindow extends Component {

	constructor(props) {
		super(props);

		this.renderTextField = this.renderTextField.bind(this);
		this.textFieldChange = this.textFieldChange.bind(this);

		this.state = { textFieldValue: "" }
	}

	renderTextField() {

		if (!this.props.renderTextField) {
			return undefined;
		}

		return (
			<div className="text-field" contentEditable suppressContentEditableWarning spellCheck="false" onInput={evt => this.textFieldChange(evt)}> </div>
		)
	}

	textFieldChange(e) {
		var newText = e.target.innerText;

		this.setState({ textFieldValue: newText });
	}


	render() {

		var textField = this.renderTextField();

		return (
			<div className="dialogue-window" >
				<div className="close" onClick={this.props.hideHandle} />
				<p className="title">{this.props.title}</p>
				<p className="sub-title">{this.props.subTitle}</p>

				{ textField }

				<div className="btns-box">
					{this.props.btns.map((e, i) => {

						var className = `dialogue-btn ${e.className}`;

						var callback = e.callback;

						if (e.sendText == true) {
							callback = () => { e.callback(this.state.textFieldValue); window.dialoguePanel.hide() };
						}
						else {
							callback = () => { e.callback(); window.dialoguePanel.hide() }
						}

						return (
							<button key={`dialogue-btn-${i}`} className={className} onClick={ callback }>{ e.text }</button>
							);
					})}
				</div>
			</div>
		);
	}

}