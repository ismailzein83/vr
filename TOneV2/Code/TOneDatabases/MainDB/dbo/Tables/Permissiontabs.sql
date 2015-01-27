CREATE TABLE [dbo].[Permissiontabs] (
    [ID]          VARCHAR (255)  NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Description] NTEXT          NULL,
    [timestamp]   ROWVERSION     NOT NULL
);

