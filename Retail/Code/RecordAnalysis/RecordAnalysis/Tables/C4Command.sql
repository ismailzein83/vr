CREATE TABLE [RecordAnalysis].[C4Command] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Type]        INT            NULL,
    [Command]     NVARCHAR (255) NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_C4Command_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_C4Command] PRIMARY KEY CLUSTERED ([Id] ASC)
);

