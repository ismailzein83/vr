CREATE TABLE [logging].[LoggableEntity] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [UniqueName]       VARCHAR (255)    NOT NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_LoggableEntity_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_LoggableEntity_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_LoggableEntity_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_LoggableEntity_Name] UNIQUE NONCLUSTERED ([UniqueName] ASC)
);



