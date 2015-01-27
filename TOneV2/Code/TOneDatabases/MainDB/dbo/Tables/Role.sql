CREATE TABLE [dbo].[Role] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Description] NTEXT          NULL,
    [IsActive]    CHAR (1)       CONSTRAINT [DF_Role_IsActive] DEFAULT ('N') NOT NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([ID] ASC)
);

