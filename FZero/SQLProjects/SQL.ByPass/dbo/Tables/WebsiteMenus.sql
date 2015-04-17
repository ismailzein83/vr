CREATE TABLE [dbo].[WebsiteMenus] (
    [ID]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50)   NOT NULL,
    [URL]          NVARCHAR (250) NULL,
    [IsActive]     BIT            CONSTRAINT [DF_WebsiteMenus_IsActive] DEFAULT ((1)) NOT NULL,
    [AppType]      INT            NULL,
    [MainMenuID]   INT            NULL,
    [Ordering]     INT            NOT NULL,
    [PermissionID] INT            NULL,
    CONSTRAINT [PK_WebsiteMenus] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_WebsiteMenus_Permissions] FOREIGN KEY ([PermissionID]) REFERENCES [dbo].[Permissions] ([ID]),
    CONSTRAINT [FK_WebsiteMenus_WebsiteMenus] FOREIGN KEY ([MainMenuID]) REFERENCES [dbo].[WebsiteMenus] ([ID])
);

