app.constant('CaseStatusEnum', {
    Open: { value: 1, description: "Open" },
    Pending: { value: 2, description: "Pending" },
    ClosedFraud: { value: 3, description: "Closed: Fraud" },
    ClosedWhitelist: { value: 4, description: "Closed: White List" }
});

app.constant('KindEnum', {
    UserDefined: { value: false, name: 'User defined' }, 
    SystemBuiltIn: { value: true, name: 'System built in' }
});

app.constant('OperatorTypeEnum', {
    Mobile: { value: 1, name: 'Mobile' },
    PSTN: { value: 2, name: 'PSTN' },
    Both: { value: 0, name: 'Both' }
});

app.constant('StatusEnum', {
    Disabled: { value: false, name: 'Disabled' },
    Enabled: { value: true, name: 'Enabled' }
});

app.constant('HourEnum', {
    h_0: { id: 0, name: '12:00 AM' }, h_1 : { id: 1, name: '01:00 AM' }, h_2 :{ id: 2, name: '02:00 AM' }, h_3 :{ id: 3, name: '03:00 AM' }, h_4 :{ id: 4, name: '04:00 AM' }, h_5 :{ id: 5, name: '05:00 AM' },
    h_6 : { id: 6, name: '06:00 AM' }, h_7 :{ id: 7, name: '07:00 AM' }, h_8 :{ id: 8, name: '08:00 AM' }, h_9 :{ id: 9, name: '09:00 AM' }, h_10 :{ id: 10, name: '10:00 AM' },h_11 : { id: 11, name: '11:00 AM' },
    h_12 :{ id: 12, name: '12:00 PM' },h_13 : { id: 13, name: '01:00 PM' },h_14 : { id: 14, name: '02:00 PM' }, h_15 : { id: 15, name: '03:00 PM' },h_16 : { id: 16, name: '04:00 PM' }, h_17 : { id: 17, name: '05:00 PM' },
    h_18 : { id: 18, name: '06:00 PM' },h_19 : { id: 19, name: '07:00 PM' }, h_20 :{ id: 20, name: '08:00 PM' }, h_21 :{ id: 21, name: '09:00 PM' }, h_22 : { id: 22, name: '10:00 PM' }, h_23 :{ id: 23, name: '11:00 PM' }
});

app.constant('PercentageEnum', {
    SeventyFivePlus: { description: '75%', value: 1.75 },
    FiftyPlus:  { description: '50%', value: 1.50 },
    TwentyfivePlus: { description: '25%', value: 1.25 },
    Zero : { description: '0%', value: 1.00 },
    TwentyFiveMinus: { description: '-25%', value: 0.75 },
    FiftyMinus: { description: '-50%', value: 0.5 },
    SeventyFiveMinus: { description: '-75%', value: 0.25 }
});


app.constant("SuspicionLevelEnum", {
    Suspicious: { value: 2, description: "Suspicious" },
    HighlySuspicious: { value: 3, description: "Highly Suspicious" },
    Fraud: { value: 4, description: "Fraud" }
});

app.constant("SuspicionOccuranceStatusEnum", {
    Open: { value: 1, description: "Open" },
    Closed: { value: 10, description: "Closed" },
    Deleted: { value: 20, description: "Deleted" }
});

app.constant("CallTypeEnum", {
    NotDefined: { value: 0, description: "Not Defined" },
    OutgoingVoiceCall: { value: 1, description: "Outgoing Voice Call" },
    IncomingVoiceCall: { value: 2, description: "Incoming Voice Call" },
    CallForward: { value: 29, description: "Call Forward" },
    IncomingSms: { value: 30, description: "Incoming Sms" },
    OutgoingSms: { value: 31, description: "Outgoing Sms" },
    RoamingCallForward: { value: 26, description: "Roaming Call Forward" },
});
