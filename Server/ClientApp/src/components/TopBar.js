import React, { Component } from 'react';
import { MySignalR } from './MySignalR';
import "./TopBar.css";

export class TopBar extends Component {

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <header className="top-bar">
                <div className="container-fluid">
                    <a className="px-4 logo" href="/">NOT A RAT</a>
                    <div className="right">
                        <MySignalR></MySignalR>
                        <a className="px-4 logo center-vertically" href="/logs">Logs</a>
                        <a className="px-4 logo center-vertically" href="/logs">Settings</a>
                    </div>
                </div>
            </header>
        );
    }
}
