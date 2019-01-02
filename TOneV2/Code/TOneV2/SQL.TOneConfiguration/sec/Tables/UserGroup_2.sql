CREATE TABLE [sec].[UserGroup] (
    [UserId]           INT        NOT NULL,
    [GroupId]          INT        NOT NULL,
    [timestamp]        ROWVERSION NOT NULL,
    [LastModifiedTime] DATETIME   CONSTRAINT [DF_UserGroup_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_UserRole_1] PRIMARY KEY CLUSTERED ([GroupId] ASC, [UserId] ASC),
    CONSTRAINT [FK_UserRole_Role] FOREIGN KEY ([GroupId]) REFERENCES [sec].[Group] ([ID]) ON DELETE CASCADE
);







