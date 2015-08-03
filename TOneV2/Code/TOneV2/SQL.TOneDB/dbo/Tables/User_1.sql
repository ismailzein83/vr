CREATE TABLE [dbo].[User] (
    [ID]          INT            IDENTITY (0, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Password]    NVARCHAR (255) NOT NULL,
    [Email]       NVARCHAR (255) NULL,
    [IsActive]    CHAR (1)       CONSTRAINT [DF_User_IsActive] DEFAULT ('N') NOT NULL,
    [LastLogin]   SMALLDATETIME  NULL,
    [Description] NTEXT          NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([ID] ASC)
);

