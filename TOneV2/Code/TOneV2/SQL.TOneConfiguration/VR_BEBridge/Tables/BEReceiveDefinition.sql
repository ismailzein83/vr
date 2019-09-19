﻿CREATE TABLE [VR_BEBridge].[BEReceiveDefinition] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_BEReceiveDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_BEReceiveDefinition_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_BEReceiveDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);





