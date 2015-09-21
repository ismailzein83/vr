CREATE TABLE [PSTN_BE].[Switch] (
    [ID]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (255) NOT NULL,
    [TypeID]   INT            NULL,
    [AreaCode] VARCHAR (10)   NULL,
    CONSTRAINT [PK_Switch] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Switch_SwitchType] FOREIGN KEY ([TypeID]) REFERENCES [PSTN_BE].[SwitchType] ([ID])
);

