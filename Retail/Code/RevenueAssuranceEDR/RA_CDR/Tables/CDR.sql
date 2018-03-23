CREATE TABLE [RA_CDR].[CDR] (
    [ID]                 BIGINT           NULL,
    [DataSourceID]       UNIQUEIDENTIFIER NULL,
    [ProbeID]            BIGINT           NULL,
    [IDOnSwitch]         VARCHAR (255)    NULL,
    [AttemptDateTime]    DATETIME         NULL,
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
    [QueueItemId]        BIGINT           NULL
);

