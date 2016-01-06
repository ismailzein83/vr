CREATE TABLE [bp].[BPTaskType] (
    [ID]       INT            NOT NULL,
    [Name]     VARCHAR (255)  NOT NULL,
    [Settings] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_TaskType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

