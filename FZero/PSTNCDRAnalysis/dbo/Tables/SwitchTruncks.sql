CREATE TABLE [dbo].[SwitchTruncks] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [SwitchId]    INT            NOT NULL,
    [TrunckId]    INT            NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [FullName]    NVARCHAR (250) NOT NULL,
    [DirectionId] INT            NOT NULL,
    CONSTRAINT [PK_Switch_Truncks] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Switches_Truncks_SwitchProfiles] FOREIGN KEY ([SwitchId]) REFERENCES [dbo].[SwitchProfiles] ([Id]),
    CONSTRAINT [FK_Switches_Truncks_Truncks] FOREIGN KEY ([TrunckId]) REFERENCES [dbo].[Truncks] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SwitchTruncks_Directions] FOREIGN KEY ([DirectionId]) REFERENCES [dbo].[Directions] ([Id])
);

