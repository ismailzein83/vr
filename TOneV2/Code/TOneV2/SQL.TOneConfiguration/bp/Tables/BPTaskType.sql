CREATE TABLE [bp].[BPTaskType] (
    [ID]               UNIQUEIDENTIFIER CONSTRAINT [DF__BPTaskType__ID__1F5986F6] DEFAULT (newid()) NOT NULL,
    [DevProjectID]     UNIQUEIDENTIFIER NULL,
    [Name]             VARCHAR (255)    NOT NULL,
    [Title]            VARCHAR (255)    NULL,
    [Settings]         NVARCHAR (MAX)   NOT NULL,
    [timestamp]        ROWVERSION       NOT NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_BPTaskType_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [pk_BPTaskType] PRIMARY KEY CLUSTERED ([ID] ASC)
);













