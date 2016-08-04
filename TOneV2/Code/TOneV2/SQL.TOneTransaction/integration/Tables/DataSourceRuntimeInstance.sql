CREATE TABLE [integration].[DataSourceRuntimeInstance] (
    [ID]                UNIQUEIDENTIFIER NOT NULL,
    [DataSourceID]      INT              NOT NULL,
    [LockedByProcessID] INT              NULL,
    [IsCompleted]       BIT              NULL,
    [CreatedTime]       DATETIME         NULL,
    CONSTRAINT [PK_DataSourceRuntimeInstance] PRIMARY KEY CLUSTERED ([ID] ASC)
);



