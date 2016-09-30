CREATE TABLE [genericdata].[BusinessEntityDefinition] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [OldID]       INT              NULL,
    [Name]        VARCHAR (900)    NOT NULL,
    [Title]       NVARCHAR (1000)  NOT NULL,
    [Settings]    NVARCHAR (MAX)   NOT NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_BusinessEntityDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_BusinessEntityDefinition_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_BusinessEntityDefinition_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



