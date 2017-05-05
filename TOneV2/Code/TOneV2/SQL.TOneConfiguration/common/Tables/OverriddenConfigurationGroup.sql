CREATE TABLE [common].[OverriddenConfigurationGroup] (
    [ID]          UNIQUEIDENTIFIER NULL,
    [Name]        NVARCHAR (255)   NULL,
    [timestamp]   ROWVERSION       NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_OverriddenConfigGroup_CreatedTime] DEFAULT (getdate()) NULL
);

