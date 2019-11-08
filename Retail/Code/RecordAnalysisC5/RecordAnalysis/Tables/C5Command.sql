CREATE TABLE [RecordAnalysis].[C5Command] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Type]        INT            NULL,
    [Command]     NVARCHAR (255) NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_C5Command_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_C5Command] PRIMARY KEY CLUSTERED ([Id] ASC)
);

