﻿CREATE TABLE [ICX_Data].[RawData] (
    [Id]                   BIGINT           NULL,
    [RecordDateTime]       DATETIME         NULL,
    [UserSessionStartDate] DATETIME         NULL,
    [UserSession]          VARCHAR (110)    NULL,
    [UserName]             VARCHAR (50)     NULL,
    [SessionId]            VARCHAR (50)     NULL,
    [ISPName]              VARCHAR (50)     NULL,
    [InputOctets]          DECIMAL (22, 2)  NULL,
    [OutputOctets]         DECIMAL (22, 2)  NULL,
    [ExtraFields]          NVARCHAR (MAX)   NULL,
    [DataSourceId]         UNIQUEIDENTIFIER NULL,
    [QueueItemId]          BIGINT           NULL,
    [BatchIdentifier]      UNIQUEIDENTIFIER NULL
);

