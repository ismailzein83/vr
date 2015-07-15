CREATE TABLE [integration].[DataSource] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      VARCHAR (100)  NOT NULL,
    [AdapterID] INT            NOT NULL,
    [TaskId]    INT            NOT NULL,
    [Settings]  VARCHAR (1000) NOT NULL,
    CONSTRAINT [PK_DataSource] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_DataSource_AdapterType] FOREIGN KEY ([AdapterID]) REFERENCES [integration].[AdapterType] ([ID])
);

