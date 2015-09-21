CREATE TABLE [PSTN_BE].[SwitchTrunkLink] (
    [Trunk1ID] INT NOT NULL,
    [Trunk2ID] INT NOT NULL,
    CONSTRAINT [PK_SwitchTrunkLink] PRIMARY KEY CLUSTERED ([Trunk1ID] ASC, [Trunk2ID] ASC),
    CONSTRAINT [FK_SwitchTrunkLink_SwitchTrunk] FOREIGN KEY ([Trunk1ID]) REFERENCES [PSTN_BE].[SwitchTrunk] ([ID]),
    CONSTRAINT [FK_SwitchTrunkLink_SwitchTrunk1] FOREIGN KEY ([Trunk2ID]) REFERENCES [PSTN_BE].[SwitchTrunk] ([ID])
);

