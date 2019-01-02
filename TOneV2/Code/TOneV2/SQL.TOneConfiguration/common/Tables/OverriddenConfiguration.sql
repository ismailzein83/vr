﻿CREATE TABLE [common].[OverriddenConfiguration] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [GroupId]          UNIQUEIDENTIFIER NOT NULL,
    [Settings]         NVARCHAR (MAX)   NOT NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_OverriddenConfiguration_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_OverriddenConfiguration_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_OverriddenConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_OverriddenConfiguration_OverriddenConfigurationGroup] FOREIGN KEY ([GroupId]) REFERENCES [common].[OverriddenConfigurationGroup] ([ID])
);





