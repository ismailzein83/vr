CREATE TABLE [dbo].[ZGM_ZoneGroup] (
    [GroupID]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (255) NOT NULL,
    [CustomerID] VARCHAR (5)    NULL,
    [UserID]     INT            NOT NULL,
    [timestamp]  ROWVERSION     NOT NULL,
    [Updated]    DATETIME       NULL,
    CONSTRAINT [PK_ZoneGroup] PRIMARY KEY CLUSTERED ([GroupID] ASC)
);

