CREATE TABLE [RecordAnalysis].[C5Command] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [Type]        INT            NULL,
    [Command]     NVARCHAR (255) NULL,
    [CreatedTime] DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

