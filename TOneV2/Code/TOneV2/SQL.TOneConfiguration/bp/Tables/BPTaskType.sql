CREATE TABLE [bp].[BPTaskType] (
    [Name]      VARCHAR (255)    NOT NULL,
    [Settings]  NVARCHAR (MAX)   NOT NULL,
    [timestamp] ROWVERSION       NOT NULL,
    [OldID]     INT              NULL,
    [ID]        UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    CONSTRAINT [pk_BPTaskType] PRIMARY KEY CLUSTERED ([ID] ASC)
);



