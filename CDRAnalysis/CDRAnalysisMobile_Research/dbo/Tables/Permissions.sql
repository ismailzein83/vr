CREATE TABLE [dbo].[Permissions] (
    [ID]          INT            NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (500) NULL,
    [Category]    NVARCHAR (50)  NULL,
    [ParentID]    INT            NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Permissions_Permissions] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[Permissions] ([ID])
);

