CREATE TABLE [RA_ICX_CDR].[CDR] (
    [ID]                   BIGINT           NULL,
    [OperatorID]           INT              NULL,
    [DataSourceID]         UNIQUEIDENTIFIER NULL,
    [ProbeID]              BIGINT           NULL,
    [IDOnSwitch]           VARCHAR (255)    NULL,
    [AttemptDateTime]      DATETIME         NULL,
    [ConnectDateTime]      DATETIME         NULL,
    [DisconnectDateTime]   DATETIME         NULL,
    [DurationInSeconds]    DECIMAL (20, 4)  NULL,
    [CGPN]                 VARCHAR (40)     NULL,
    [CDPN]                 VARCHAR (40)     NULL,
    [ExtraFields]          NVARCHAR (MAX)   NULL,
    [Trunk]                NVARCHAR (MAX)   NULL,
    [IP]                   NVARCHAR (MAX)   NULL,
    [CauseToReleaseCode]   NVARCHAR (MAX)   NULL,
    [CauseFromReleaseCode] NVARCHAR (MAX)   NULL,
    [DisconnectReason]     VARCHAR (255)    NULL,
    [AlertDateTime]        DATETIME         NULL,
    [TrafficDirection]     INT              NULL,
    [QueueItemId]          BIGINT           NULL
);



