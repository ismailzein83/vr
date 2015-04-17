CREATE TABLE [dbo].[UserPermissions] (
    [ID]           INT           IDENTITY (1, 1) NOT NULL,
    [UserID]       INT           NOT NULL,
    [PermissionID] INT           NOT NULL,
    [CreationDate] DATETIME2 (0) CONSTRAINT [DF_UserPermissions_CreationDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]    INT           NOT NULL,
    CONSTRAINT [PK_UserPermissions_1] PRIMARY KEY CLUSTERED ([UserID] ASC, [PermissionID] ASC),
    CONSTRAINT [FK_CustomerPermissions_Customers] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_CustomerPermissions_Permissions] FOREIGN KEY ([PermissionID]) REFERENCES [dbo].[Permissions] ([ID]),
    CONSTRAINT [FK_UserPermissions_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([ID])
);

