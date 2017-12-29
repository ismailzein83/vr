CREATE TABLE [integration].[DataSourceRuntimeInstance] (
    [ID]           UNIQUEIDENTIFIER NOT NULL,
    [DataSourceID] UNIQUEIDENTIFIER NOT NULL,
    [CreatedTime]  DATETIME         CONSTRAINT [DF_DataSourceRuntimeInstance_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_DataSourceRuntimeInstance] PRIMARY KEY CLUSTERED ([ID] ASC)
);









