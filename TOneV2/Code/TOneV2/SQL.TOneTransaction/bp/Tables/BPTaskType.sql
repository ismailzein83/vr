CREATE TABLE [bp].[BPTaskType] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      VARCHAR (255)  NOT NULL,
    [Settings]  NVARCHAR (MAX) NOT NULL,
    [timestamp] ROWVERSION     NOT NULL,
    CONSTRAINT [PK_TaskType] PRIMARY KEY CLUSTERED ([ID] ASC)
);



