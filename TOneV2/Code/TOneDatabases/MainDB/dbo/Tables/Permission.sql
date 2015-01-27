CREATE TABLE [dbo].[Permission] (
    [ID]          VARCHAR (255)  NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Description] NTEXT          NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([ID] ASC)
);

