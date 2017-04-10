CREATE TABLE [dataparser].[ParserType] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [timestamp]   ROWVERSION       NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_ParserType_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ParserType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_ParserType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

