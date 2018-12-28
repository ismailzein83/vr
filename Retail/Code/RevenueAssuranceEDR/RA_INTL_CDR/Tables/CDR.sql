﻿CREATE TABLE [RA_INTL_CDR].[CDR] (
    [ID]                   BIGINT           NULL,
    [OperatorID]           INT              NULL,
    [DataSourceID]         UNIQUEIDENTIFIER NULL,
    [ProbeID]              BIGINT           NULL,
    [IDOnSwitch]           VARCHAR (255)    NULL,
    [AttemptDateTime]      DATETIME         NULL,
    [ConnectDateTime]      DATETIME         NULL,
    [DisconnectDateTime]   DATETIME         NULL,
    [CGPN]                 VARCHAR (40)     NULL,
    [CDPN]                 VARCHAR (40)     NULL,
    [ExtraFields]          NVARCHAR (MAX)   NULL,
    [Trunk]                NVARCHAR (MAX)   NULL,
    [IP]                   NVARCHAR (MAX)   NULL,
    [CauseToReleaseCode]   NVARCHAR (MAX)   NULL,
    [CauseFromReleaseCode] NVARCHAR (MAX)   NULL,
    [AlertDateTime]        DATETIME         NULL,
    [DisconnectReason]     NVARCHAR (MAX)   NULL,
    [TrafficDirection]     INT              NULL,
    [DurationInSeconds]    DECIMAL (20, 4)  NULL,
    [QueueItemId]          BIGINT           NULL
);



