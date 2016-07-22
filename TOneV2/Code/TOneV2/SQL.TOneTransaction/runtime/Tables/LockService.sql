CREATE TABLE [runtime].[LockService] (
    [ID]                INT            NOT NULL,
    [ServiceURL]        VARCHAR (255)  NULL,
    [LockingDetails]    NVARCHAR (MAX) NULL,
    [LockedByProcessID] INT            NULL,
    [LastLockedTime]    DATETIME       NULL,
    [LastUpdatedTime]   DATETIME       NULL,
    CONSTRAINT [PK_LockManager] PRIMARY KEY CLUSTERED ([ID] ASC)
);

