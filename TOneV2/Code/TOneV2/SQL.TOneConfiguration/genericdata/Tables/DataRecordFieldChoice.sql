﻿CREATE TABLE [genericdata].[DataRecordFieldChoice] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_DataRecordFieldChoice_CreateTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_DataRecordFieldChoice_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_DataRecordFieldChoice_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);









