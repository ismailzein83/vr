CREATE TABLE [RecordAnalysis].[C4Probe] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (255)  NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [Settings]         NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_C4Probe] PRIMARY KEY CLUSTERED ([Id] ASC)
);

