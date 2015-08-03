CREATE TABLE [dbo].[Users] (
    [ID]          INT            IDENTITY (0, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Password]    NVARCHAR (255) NOT NULL,
    [Email]       NVARCHAR (255) NULL,
    [IsActive]    CHAR (1)       CONSTRAINT [DF_Users_IsActive] DEFAULT ('N') NOT NULL,
    [LastLogin]   SMALLDATETIME  NULL,
    [Description] NTEXT          NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([ID] ASC)
);

