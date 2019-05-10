﻿CREATE TABLE [Analytic].[AnalyticTable] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             VARCHAR (255)    NOT NULL,
    [Settings]         NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_AnalyticTable_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_AnalyticTable_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [MeasureStyles]    NVARCHAR (MAX)   NULL,
    [PermanentFilter]  NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_AnalyticTable] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_AnalyticTable_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);











