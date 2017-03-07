CREATE TABLE [logging].[LoggableEntity] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [UniqueName]  VARCHAR (255)  NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_LoggableEntity_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_LoggableEntity] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_LoggableEntity_Name] UNIQUE NONCLUSTERED ([UniqueName] ASC)
);

