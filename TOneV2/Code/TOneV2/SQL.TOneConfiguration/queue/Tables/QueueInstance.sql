CREATE TABLE [queue].[QueueInstance] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (255)  NOT NULL,
    [ExecutionFlowID] INT            NULL,
    [StageName]       VARCHAR (255)  NULL,
    [Title]           NVARCHAR (255) NOT NULL,
    [Status]          INT            NOT NULL,
    [ItemTypeID]      INT            NOT NULL,
    [Settings]        NVARCHAR (MAX) NULL,
    [CreatedTime]     DATETIME       CONSTRAINT [DF_Queue_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]       ROWVERSION     NULL,
    CONSTRAINT [PK_Queue] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_QueueInstance_ExecutionFlow] FOREIGN KEY ([ExecutionFlowID]) REFERENCES [queue].[ExecutionFlow] ([ID]),
    CONSTRAINT [FK_QueueInstance_QueueItemType] FOREIGN KEY ([ItemTypeID]) REFERENCES [queue].[QueueItemType] ([ID])
);

