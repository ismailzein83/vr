CREATE TYPE [RA_INTL_CDR].[CDRType] AS TABLE (
    [ID]                   BIGINT           NULL,
    [OperatorID]           INT              NULL,
    [TrafficDirection]     INT              NULL,
    [DataSourceID]         UNIQUEIDENTIFIER NULL,
    [ProbeID]              BIGINT           NULL,
    [IDOnSwitch]           VARCHAR (255)    NULL,
    [AttemptDateTime]      DATETIME         NULL,
    [ConnectDateTime]      DATETIME         NULL,
    [DisconnectDateTime]   DATETIME         NULL,
    [DisconnectReason]     NVARCHAR (MAX)   NULL,
    [AlertDateTime]        DATETIME         NULL,
    [DurationInSeconds]    DECIMAL (20, 4)  NULL,
    [CDPN]                 VARCHAR (40)     NULL,
    [CGPN]                 VARCHAR (40)     NULL,
    [CauseFromReleaseCode] NVARCHAR (MAX)   NULL,
    [CauseToReleaseCode]   NVARCHAR (MAX)   NULL,
    [Trunk]                NVARCHAR (MAX)   NULL,
    [IP]                   NVARCHAR (MAX)   NULL,
    [ExtraFields]          NVARCHAR (MAX)   NULL,
    [QueueItemId]          BIGINT           NULL);



