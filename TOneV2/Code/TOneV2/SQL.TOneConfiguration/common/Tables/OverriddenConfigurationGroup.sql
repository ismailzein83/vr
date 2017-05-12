CREATE TABLE [common].[OverriddenConfigurationGroup] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NULL,
    [timestamp]   ROWVERSION       NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_OverriddenConfigGroup_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_OverriddenConfigurationGroup] PRIMARY KEY CLUSTERED ([ID] ASC)
);



