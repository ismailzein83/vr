CREATE TABLE [RA_ICX_CDR].[CDR] (
    [ID]                   BIGINT           NOT NULL,
    [OperatorID]           INT              NULL,
    [DataSourceID]         UNIQUEIDENTIFIER NULL,
    [ProbeID]              BIGINT           NULL,
    [IDOnSwitch]           VARCHAR (255)    NULL,
    [AttemptDateTime]      DATETIME         NULL,
    [ConnectDateTime]      DATETIME         NULL,
    [DisconnectDateTime]   DATETIME         NULL,
    [DisconnectReason]     VARCHAR (255)    NULL,
    [AlertDateTime]        DATETIME         NULL,
    [DurationInSeconds]    DECIMAL (20, 4)  NULL,
    [CGPN]                 VARCHAR (40)     NULL,
    [CDPN]                 VARCHAR (40)     NULL,
    [CauseFromReleaseCode] NVARCHAR (MAX)   NULL,
    [CauseToReleaseCode]   NVARCHAR (MAX)   NULL,
    [Trunk]                NVARCHAR (MAX)   NULL,
    [IP]                   NVARCHAR (MAX)   NULL,
    [TrafficDirection]     INT              NULL,
    [ExtraFields]          NVARCHAR (MAX)   NULL,
    [QueueItemId]          BIGINT           NULL,
    CONSTRAINT [IX_RA_ICX_CDR_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_RA_ICX_CDR_AttemptDateTime]
    ON [RA_ICX_CDR].[CDR]([AttemptDateTime] ASC);

