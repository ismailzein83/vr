CREATE TABLE [Retail_BE].[ActionDefinition] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Settings]    NVARCHAR (MAX)   NOT NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_ActionDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_ActionDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_ActionDefinition_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

