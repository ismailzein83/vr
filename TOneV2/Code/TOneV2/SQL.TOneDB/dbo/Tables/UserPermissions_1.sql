CREATE TABLE [dbo].[UserPermissions] (
    [UserID]       INT        NOT NULL,
    [PermissionID] INT        NOT NULL,
    [timestamp]    ROWVERSION NOT NULL,
    [DS_ID_auto]   INT        IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_UserPermissions] PRIMARY KEY CLUSTERED ([UserID] ASC, [PermissionID] ASC),
    CONSTRAINT [fk_UP_ID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[User] ([ID])
);

