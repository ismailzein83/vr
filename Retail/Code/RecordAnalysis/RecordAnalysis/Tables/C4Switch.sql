CREATE TABLE [RecordAnalysis].[C4Switch] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (255)  NULL,
    [Settings]         NVARCHAR (MAX) NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK_C4Switch] PRIMARY KEY CLUSTERED ([Id] ASC)
);

