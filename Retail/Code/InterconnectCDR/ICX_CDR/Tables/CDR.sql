CREATE TABLE [ICX_CDR].[CDR] (
    [ID]                 BIGINT           NOT NULL,
    [DataSourceID]       UNIQUEIDENTIFIER NOT NULL,
    [SwitchId]           INT              NOT NULL,
    [IDOnSwitch]         VARCHAR (255)    NULL,
    [AttemptDateTime]    DATETIME         NULL,
    [AlertDateTime]      DATETIME         NULL,
    [ConnectDateTime]    DATETIME         NULL,
    [DisconnectDateTime] DATETIME         NULL,
    [DurationInSeconds]  DECIMAL (20, 4)  NULL,
    [CGPN]               VARCHAR (40)     NULL,
    [CDPN]               VARCHAR (40)     NULL,
    [ReleaseCode]        VARCHAR (50)     NULL,
    [InTrunk]            VARCHAR (50)     NULL,
    [InIP]               VARCHAR (50)     NULL,
    [OutTrunk]           VARCHAR (50)     NULL,
    [OutIP]              VARCHAR (50)     NULL,
    [ExtraFields]        NVARCHAR (MAX)   NULL,
    [QueueItemId]        BIGINT           NULL,
    CONSTRAINT [IX_CDR_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_CDR_AttemptDateTime]
    ON [ICX_CDR].[CDR]([AttemptDateTime] ASC);

