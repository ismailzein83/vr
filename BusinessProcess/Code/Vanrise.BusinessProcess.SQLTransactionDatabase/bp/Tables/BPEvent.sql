CREATE TABLE [bp].[BPEvent] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID] UNIQUEIDENTIFIER NULL,
    [Bookmark]          VARCHAR (1000)   NOT NULL,
    [Payload]           NVARCHAR (MAX)   NULL,
    [CreatedTime]       DATETIME         CONSTRAINT [DF_BPEvent_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_BPEvent] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_BPEvent_BPInstance] FOREIGN KEY ([ProcessInstanceID]) REFERENCES [bp].[BPInstance] ([ID]) ON DELETE CASCADE
);

