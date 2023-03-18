import React, { Component } from 'react';
import { HubConnectionBuilder, signalR } from '@microsoft/signalr';




export class MySignalR extends Component {

    constructor(props) {
		super(props);
        this.state = { signalrConnection: undefined, events: this.populateEvents() };

        this.buildConnection = this.buildConnection.bind(this);
        this.startConnection = this.startConnection.bind(this);
        this.sendMessage = this.sendMessage.bind(this);
        this.addEvent = this.addEvent.bind(this);
        this.removeEvent = this.removeEvent.bind(this);

        window.MySignalR = this;
    }

    populateEvents() {
        var events = [];

        events.push({ name: "ReceiveMessage", callback: (message) => { console.log(message); } });

        events.push({
            name: 'TriggerNotification', callback: (message) => {

                var type;

                switch (message.type) {
                    case 1:
                        type = "wng";
                        break;
                    case 2:
                        type = "dng";
                        break;
                    case 3:
                        type = "suc";
                        break;
                    default:
                        type = "inf";
                        break;
                }

                window.notificationPanel.notify(message.text, type, message.url);
            }
        });

        events.push({ name: 'Refresh', callback: (message) => { window.location.reload(); } });


        return events;
    }

    addEvent(name, callback) {
        this.state.events.push({ name: name, callback: callback });

        this.forceUpdate();
    }

    removeEvent(name, callback) {

    }

    buildConnection() {
        this.state.signalrConnection = new HubConnectionBuilder()
            .withUrl("/api/generalHub",
                {
                    transport: 4 // LongPolling = 4 Specifies the Long Polling transport.
                }
            )
            .withAutomaticReconnect()
            .build();
    };

    startConnection() {
        if (this.state.signalrConnection) {

            this.state.signalrConnection.start()
                .then(result => {

                    this.state.events.forEach(e => this.state.signalrConnection.on(e.name, (message) => { e.callback(message) }));

                })
                .catch(e => console.log('Connection failed: ', e));
        }
        else {
            console.log("Cant start a connection because it doesnt exist.");
        }
    }

    sendMessage() {
        if (this.state.signalrConnection.connection._connectionStarted) {
            this.state.signalrConnection.send("SendMessage", "testMsg");
        }
        else {
            console.log("cant send because connection is not established.")
        }
    }

    render() {
        this.buildConnection();
        this.startConnection();

        return (null);
    }

}
