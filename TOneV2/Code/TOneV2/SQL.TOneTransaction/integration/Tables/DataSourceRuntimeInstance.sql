CREATE TABLE [integration].[DataSourceRuntimeInstance] (
    [ID]                UNIQUEIDENTIFIER NOT NULL,
    [DataSourceID]      UNIQUEIDENTIFIER NOT NULL,
    [OldDataSourceID]   INT              NULL,
    [LockedByProcessID] INT              NULL,
    [IsCompleted]       BIT              NULL,
    [CreatedTime]       DATETIME         NULL,
    CONSTRAINT [PK_DataSourceRuntimeInstance] PRIMARY KEY CLUSTERED ([ID] ASC)
);





