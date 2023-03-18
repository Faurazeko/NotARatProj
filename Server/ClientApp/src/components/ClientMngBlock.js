import React, { Component } from 'react';

export class ClientMngBlock extends Component {
    static displayName = ClientMngBlock.name;

    constructor(props) {
        super(props);
	}

	getUpdatedAgoString(clintUtcTime) {

		var now = new Date;
		var utc_timestamp = Date.UTC(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(),
			now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds(), now.getUTCMilliseconds());


		var ticks = utc_timestamp - clintUtcTime;

		function pad(n, width) {
			n = n + '';
			return n.length >= width ? n : new Array(width - n.length + 1).join('0') + n;
		}

		var result = "Updated ";

		var seconds = ticks / 1000;
		var dd = (Math.floor(seconds / 86400)).toFixed(0);

		seconds -= dd * 86400;

		var hh = (Math.floor(seconds / 3600)).toFixed(0);
		var mm = (Math.floor((seconds % 3600) / 60)).toFixed(0);
		var ss = (seconds % 60).toFixed(0);

		if (dd > 0) {
			result += pad(dd, 1) + " d, ";
		}

		if (hh > 0) {
			result += pad(hh, 1) + " h, ";
		}

		if (mm > 0) {
			result += pad(mm, 1) + " m, ";
		}

		return result + pad(ss, 1) + " s ago";
	}

	render() {

        return (
			<div className="item m-2">
				<div className="item-image-container">
					<h3 className="item-title px-2">{this.props.client.hostname}</h3>
					<img src={ this.props.client.wallpaperPath} className="item-image">
					</img>
					<div className={(this.props.client.isOnline ? 'status-circle status-circle-online' : 'status-circle') + " status-circle-corner"} />
				</div>

				<div className="content p-3 pt-1 text-break text-wrap">
					<h3>Client information</h3>

					<h5>
						Status -
						{
							this.props.client.isOnline ?
							<span className="text-succ ms-1">Online</span> :
							<span className="text-dng ms-1">Offline</span>
						}
					</h5>

					<h5>{this.getUpdatedAgoString(new Date(this.props.client.lastUpdateUtcTime))}</h5>

					<h5>Public IP - { this.props.client.publicIp}</h5>
					<h5>Software ver. - {this.props.client.programVersion}</h5>

					<h5>
						Cam -
						{
							this.props.client.hasCam ?
								<span className="text-succ ms-1">Yes</span> :
								<span className="text-dng ms-1">No</span>
						}
					</h5>
					<h5>
						Mic -
						{
							this.props.client.hasMic ?
								<span className="text-succ ms-1">Yes</span> :
								<span className="text-dng ms-1">No</span>
						}
					</h5>

					<div className="control-panel">
						<a href={"/client/" + this.props.client.id}>
							<button> Open </button>
						</a>
					</div>
				</div>
			</div>
        );
    }
}
