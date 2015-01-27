CREATE TABLE [dbo].[WebSiteMenuItemPermission] (
    [ItemID]       INT           NOT NULL,
    [PermissionID] VARCHAR (255) NOT NULL,
    [timestamp]    ROWVERSION    NOT NULL,
    [DS_ID_auto]   INT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_websitemenuitempermission] PRIMARY KEY CLUSTERED ([ItemID] ASC, [PermissionID] ASC),
    CONSTRAINT [FK_WebSiteMenuItemPermission_Permission] FOREIGN KEY ([PermissionID]) REFERENCES [dbo].[Permission] ([ID]),
    CONSTRAINT [FK_websitemenuitempermission_WebsiteMenuItem] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[WebsiteMenuItem] ([ItemID])
);

