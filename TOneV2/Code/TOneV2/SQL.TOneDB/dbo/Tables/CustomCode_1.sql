CREATE TABLE [dbo].[CustomCode] (
    [ID]          INT            NOT NULL,
    [Description] NVARCHAR (255) NULL,
    [FQTN]        VARCHAR (1000) NULL,
    [Code]        NTEXT          NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_CustomCode_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_CustomCode] PRIMARY KEY CLUSTERED ([ID] ASC)
);

