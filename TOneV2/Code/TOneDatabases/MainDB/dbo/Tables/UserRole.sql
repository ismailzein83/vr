CREATE TABLE [dbo].[UserRole] (
    [User]       INT        NOT NULL,
    [Role]       INT        NOT NULL,
    [timestamp]  ROWVERSION NULL,
    [DS_ID_auto] INT        IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([User] ASC, [Role] ASC),
    CONSTRAINT [FK_UserRole_Role] FOREIGN KEY ([Role]) REFERENCES [dbo].[Role] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRole_User] FOREIGN KEY ([User]) REFERENCES [dbo].[User] ([ID]) ON DELETE CASCADE
);

