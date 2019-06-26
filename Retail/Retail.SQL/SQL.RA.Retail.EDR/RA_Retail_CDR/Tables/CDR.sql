CREATE TABLE [RA_Retail_CDR].[CDR] (
    [ID]                 BIGINT           NOT NULL,
    [ProbeID]            BIGINT           NULL,
    [TrafficDirection]   INT              NULL,
    [AttemptDateTime]    DATETIME         NULL,
    [ConnectDateTime]    DATETIME         NULL,
    [DisconnectDateTime] DATETIME         NULL,
    [AlertDateTime]      DATETIME         NULL,
    [DurationInSeconds]  DECIMAL (20, 4)  NULL,
    [CGPN]               NVARCHAR (255)   NULL,
    [CDPN]               NVARCHAR (255)   NULL,
    [Trunk]              NVARCHAR (255)   NULL,
    [SubscriberType]     INT              NULL,
    [OperatorID]         BIGINT           NULL,
    [DataSourceID]       UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__Postpaid__3214EC275BE2A6F2] PRIMARY KEY CLUSTERED ([ID] ASC)
);

