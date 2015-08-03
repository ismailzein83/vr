CREATE TABLE [dbo].[WebsiteMenuItem] (
    [ItemID]          INT            IDENTITY (1, 1) NOT NULL,
    [Path]            VARCHAR (100)  NOT NULL,
    [Text]            VARCHAR (255)  NOT NULL,
    [Description]     VARCHAR (1024) NULL,
    [NavigateURL]     VARCHAR (512)  NULL,
    [timestamp]       ROWVERSION     NOT NULL,
    [IsVisibleInMenu] BIT            CONSTRAINT [DF_WebsiteMenuItem_IsVisibleInMenu] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_WebsiteMenu] PRIMARY KEY CLUSTERED ([ItemID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_WebsiteMenu]
    ON [dbo].[WebsiteMenuItem]([Path] ASC);

