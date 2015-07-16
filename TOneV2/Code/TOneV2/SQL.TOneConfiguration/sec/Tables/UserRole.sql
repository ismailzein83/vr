CREATE TABLE [sec].[UserRole] (
    [UserID] INT NOT NULL,
    [RoleID] INT NOT NULL,
    CONSTRAINT [PK_UserRole_1] PRIMARY KEY CLUSTERED ([RoleID] ASC, [UserID] ASC),
    CONSTRAINT [FK_UserRole_Role] FOREIGN KEY ([RoleID]) REFERENCES [sec].[Role] ([ID]),
    CONSTRAINT [FK_UserRole_User] FOREIGN KEY ([UserID]) REFERENCES [sec].[User] ([ID])
);

