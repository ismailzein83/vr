CREATE TYPE [RA_ICX_CDR].[BillingCDR_ChargedType] AS TABLE (
    [CDRID]              BIGINT           NULL,
    [DataSourceID]       UNIQUEIDENTIFIER NULL,
    [ProbeID]            BIGINT           NULL,
    [IDOnSwitch]         VARCHAR (255)    NULL,
    [AttemptDateTime]    DATETIME         NULL,
    [ConnectDateTime]    DATETIME         NULL,
    [DisconnectDateTime] DATETIME         NULL,
    [DurationInSeconds]  DECIMAL (20, 4)  NULL,
    [CDPN]               VARCHAR (40)     NULL,
    [CGPN]               VARCHAR (40)     NULL,
    [OperatorID]         BIGINT           NULL,
    [TrafficDirection]   INT              NULL,
    [QueueItemId]        BIGINT           NULL);

