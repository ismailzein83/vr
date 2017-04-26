﻿CREATE TABLE [genericdata].[DataRecordType] (
    [ID]                   UNIQUEIDENTIFIER NOT NULL,
    [Name]                 NVARCHAR (1000)  NOT NULL,
    [ParentID]             UNIQUEIDENTIFIER NULL,
    [Fields]               NVARCHAR (MAX)   NULL,
    [ExtraFieldsEvaluator] NVARCHAR (MAX)   NULL,
    [Settings]             NVARCHAR (MAX)   NULL,
    [CreatedTime]          DATETIME         CONSTRAINT [DF_DataRecordType_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]            ROWVERSION       NULL,
    CONSTRAINT [PK_DataRecordType_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);









