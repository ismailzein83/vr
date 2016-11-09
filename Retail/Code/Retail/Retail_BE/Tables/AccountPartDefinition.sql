CREATE TABLE [Retail_BE].[AccountPartDefinition] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [OldID]       INT              NULL,
    [Title]       NVARCHAR (255)   NULL,
    [Name]        NVARCHAR (255)   NULL,
    [Details]     NVARCHAR (MAX)   NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_AccountPartDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_AccountPartDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_AccountPartDefinition_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



