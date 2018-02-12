﻿CREATE TABLE [TOneWhS_Case].[CaseReason] (
    [ID]               UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL
);



