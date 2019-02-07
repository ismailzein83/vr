CREATE TABLE [ICX_CDR].[BillingCDR_Interconnect] (
    [CDRID]              BIGINT           NULL,
    [DataSourceID]       UNIQUEIDENTIFIER NULL,
    [SwitchID]           INT              NULL,
    [AttemptDateTime]    DATETIME         NULL,
    [AlertDateTime]      DATETIME         NULL,
    [ConnectDateTime]    DATETIME         NULL,
    [DisconnectDateTime] DATETIME         NULL,
    [DurationInSeconds]  DECIMAL (20, 4)  NULL,
    [PDDInSeconds]       DECIMAL (20, 4)  NULL,
    [CDPN]               VARCHAR (40)     NULL,
    [ReleaseCode]        VARCHAR (50)     NULL,
    [CGPN]               VARCHAR (40)     NULL,
    [OperatorTypeID]     UNIQUEIDENTIFIER NULL,
    [OperatorID]         BIGINT           NULL,
    [TrafficDirection]   INT              NULL,
    [BillingType]        INT              NULL,
    [OriginationZoneID]  BIGINT           NULL,
    [DestinationZoneID]  BIGINT           NULL,
    [CallType]           INT              NULL,
    [CDRType]            INT              NULL,
    [CallingOperatorId]  BIGINT           NULL,
    [CalledOperatorId]   BIGINT           NULL,
    CONSTRAINT [IX_BillingCDR_Interconnect_CDRID] UNIQUE NONCLUSTERED ([CDRID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_BillingCDR_Interconnect_Attempt_Operator]
    ON [ICX_CDR].[BillingCDR_Interconnect]([AttemptDateTime] ASC, [OperatorID] ASC);

