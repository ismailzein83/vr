﻿CREATE TABLE [common].[Setting] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Type]             NVARCHAR (255)   NOT NULL,
    [Category]         VARCHAR (255)    NOT NULL,
    [Settings]         NVARCHAR (MAX)   NOT NULL,
    [Data]             NVARCHAR (MAX)   NULL,
    [IsTechnical]      BIT              NULL,
    [timestamp]        ROWVERSION       NOT NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_Setting_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_common.Setting] PRIMARY KEY CLUSTERED ([ID] ASC)
);











