CREATE TABLE [RecordAnalysis].[C4Interconnection] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (255) NULL,
    [CreatedBy]        INT           NULL,
    [CreatedTime]      DATETIME      NULL,
    [LastModifiedBy]   INT           NULL,
    [LastModifiedTime] DATETIME      NULL,
    [timestamp]        ROWVERSION    NULL,
    CONSTRAINT [PK_InterConnection] PRIMARY KEY CLUSTERED ([Id] ASC)
);

