CREATE TABLE [RA_Retail_CDR].[PostpaidCDR_Invalid] (
    [ID]                 BIGINT           NOT NULL,
    [OperatorID]         BIGINT           NULL,
    [DataSourceID]       UNIQUEIDENTIFIER NULL,
    [ProbeID]            BIGINT           NULL,
    [AttemptDateTime]    DATETIME         NULL,
    [ConnectDateTime]    DATETIME         NULL,
    [DisconnectDateTime] DATETIME         NULL,
    [DurationInSeconds]  DECIMAL (20, 4)  NULL,
    [CDPN]               NVARCHAR (255)   NULL,
    [CGPN]               NVARCHAR (255)   NULL,
    [TrafficDirection]   INT              NULL,
    [AlertDateTime]      DATETIME         NULL,
    [PDDInSeconds]       DECIMAL (20, 4)  NULL,
    [SubscriberID]       BIGINT           NULL,
    [ServiceTypeID]      UNIQUEIDENTIFIER NULL,
    [CDRType]            INT              NULL,
    [OriginationZone]    BIGINT           NULL,
    [DestinationZone]    BIGINT           NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

