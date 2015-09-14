
app.constant("CallTypeEnum", {
    NotDefined: { value: 0, description: "Not Defined" },
    OutgoingVoiceCall: { value: 1, description: "Outgoing Voice Call" },
    IncomingVoiceCall: { value: 2, description: "Incoming Voice Call" },
    CallForward: { value: 29, description: "Call Forward" },
    IncomingSms: { value: 30, description: "Incoming Sms" },
    OutgoingSms: { value: 31, description: "Outgoing Sms" },
    RoamingCallForward: { value: 26, description: "Roaming Call Forward" },
});
