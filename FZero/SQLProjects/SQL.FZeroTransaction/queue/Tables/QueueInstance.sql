CREATE TABLE [queue].[QueueInstance] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (255)  NOT NULL,
    [Title]       NVARCHAR (255) NOT NULL,
    [Status]      INT            NOT NULL,
    [ItemTypeID]  INT            NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_Queue_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Queue] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QueueInstance_Name]
    ON [queue].[QueueInstance]([Name] ASC);

