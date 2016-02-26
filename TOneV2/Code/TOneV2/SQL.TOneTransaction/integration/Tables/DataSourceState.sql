CREATE TABLE [integration].[DataSourceState] (
    [DataSourceID]      INT            NOT NULL,
    [State]             NVARCHAR (MAX) NULL,
    [LockedByProcessID] INT            NULL,
    CONSTRAINT [PK_DataSourceState] PRIMARY KEY CLUSTERED ([DataSourceID] ASC)
);

