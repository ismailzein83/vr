CREATE TABLE [integration].[AdapterType] (
    [ID]   INT            NOT NULL,
    [Name] VARCHAR (50)   NOT NULL,
    [Info] VARCHAR (1000) NOT NULL,
    CONSTRAINT [PK_AdapterType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

