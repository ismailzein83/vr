CREATE TABLE [dbo].[Switch_DatabaseConnections] (
    [Id]           INT          NOT NULL,
    [ServerName]   VARCHAR (50) NOT NULL,
    [DatabaseName] VARCHAR (50) NOT NULL,
    [UserId]       VARCHAR (50) NOT NULL,
    [UserPassword] VARCHAR (50) NOT NULL,
    CONSTRAINT [FK_ID] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Switch_DatabaseConnections_Switchs] FOREIGN KEY ([Id]) REFERENCES [dbo].[SwitchProfiles] ([Id]) ON DELETE CASCADE
);

