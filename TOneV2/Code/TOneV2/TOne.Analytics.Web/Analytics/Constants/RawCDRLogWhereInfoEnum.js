﻿app.constant('RawCDRLogWhereInfoEnum', {
    CDRID: { value: 0, column: "CDRID", type: "[bigint] IDENTITY(1,1) NOT NULL" },
    SwitchID: { value: 1, column: "SwitchID", type: "[tinyint] NOT NULL" },
    IDonSwitch: { value: 2, column: "IDonSwitch", type: "[bigint] NULL" },
    Tag: { value: 2, column: "Tag", type: "[varchar](30) NULL" },
    AttemptDateTime: { value: 2, column: "AttemptDateTime", type: "[datetime] NULL" },
    AlertDateTime: { value: 2, column: "AlertDateTime", type: "[datetime] NULL" },
    ConnectDateTime: { value: 2, column: "ConnectDateTime", type: "[datetime] NULL" },
    DisconnectDateTime: { value: 2, column: "DisconnectDateTime", type: "[datetime] NULL" },
    DurationInSeconds: { value: 2, column: "DurationInSeconds", type: "[numeric](13, 4) NULL" },
    IN_TRUNK: { value: 2, column: "IN_TRUNK", type: "[bigint] NULL" },
    IN_CIRCUIT: { value: 2, column: "IN_CIRCUIT", type: "[smallint] NULL" },
    IN_CARRIER: { value: 2, column: "IN_CARRIER", type: "[varchar](100) NULL" },
    IN_IP: { value: 2, column: "IN_IP", type: "[varchar](21) NULL" },
    OUT_TRUNK: { value: 2, column: "OUT_TRUNK", type: "[varchar](5) NULL" },
    OUT_CIRCUIT: { value: 2, column: "OUT_CIRCUIT", type: "[smallint] NULL" },
    OUT_CARRIER: { value: 2, column: "OUT_CARRIER", type: "[varchar](100) NULL" },
    OUT_IP: { value: 2, column: "OUT_IP", type: "[varchar](21) NULL" },
    CGPN: { value: 2, column: "CGPN", type: "[varchar](40) NULL" },
    CDPN: { value: 2, column: "CDPN", type: "[varchar](40) NULL" },
    CAUSE_FROM_RELEASE_CODE: { value: 2, column: "CAUSE_FROM_RELEASE_CODE", type: "[varchar](20) NULL" },
    CAUSE_FROM: { value: 2, column: "CAUSE_FROM", type: "[varchar](10) NULL" },
    CAUSE_TO_RELEASE_CODE: { value: 2, column: "CAUSE_TO_RELEASE_CODE", type: "[varchar](20) NULL" },
    CAUSE_TO: { value: 2, column: "CAUSE_TO", type: "[varchar](10) NULL" },
    Extra_Fields: { value: 2, column: "Extra_Fields", type: "[varchar](255) NULL" },
	
});