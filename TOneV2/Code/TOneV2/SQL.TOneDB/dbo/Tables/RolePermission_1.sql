CREATE TABLE [dbo].[RolePermission] (
    [Role]       INT           NOT NULL,
    [Permission] VARCHAR (255) NOT NULL,
    [timestamp]  ROWVERSION    NULL,
    [DS_ID_auto] INT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_RolePermission] PRIMARY KEY CLUSTERED ([Role] ASC, [Permission] ASC),
    CONSTRAINT [FK_RolePermission_Permission] FOREIGN KEY ([Permission]) REFERENCES [dbo].[Permission] ([ID]),
    CONSTRAINT [FK_RolePermission_Role] FOREIGN KEY ([Role]) REFERENCES [dbo].[Role] ([ID]) ON DELETE CASCADE
);

