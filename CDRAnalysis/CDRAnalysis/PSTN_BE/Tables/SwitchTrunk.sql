CREATE TABLE [PSTN_BE].[SwitchTrunk] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [SwitchID]  INT            NOT NULL,
    [Symbol]    NVARCHAR (50)  NOT NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [Direction] INT            NOT NULL,
    [Type]      INT            NOT NULL,
    CONSTRAINT [PK_SwitchTrunk] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SwitchTrunk_Switch] FOREIGN KEY ([SwitchID]) REFERENCES [PSTN_BE].[Switch] ([ID])
);



