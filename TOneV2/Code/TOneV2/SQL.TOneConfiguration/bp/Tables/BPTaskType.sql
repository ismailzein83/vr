CREATE TABLE [bp].[BPTaskType] (
    [ID]        UNIQUEIDENTIFIER CONSTRAINT [DF__BPTaskType__ID__1F5986F6] DEFAULT (newid()) NOT NULL,
    [OldID]     INT              NULL,
    [Name]      VARCHAR (255)    NOT NULL,
    [Settings]  NVARCHAR (MAX)   NOT NULL,
    [timestamp] ROWVERSION       NOT NULL,
    CONSTRAINT [pk_BPTaskType] PRIMARY KEY CLUSTERED ([ID] ASC)
);





