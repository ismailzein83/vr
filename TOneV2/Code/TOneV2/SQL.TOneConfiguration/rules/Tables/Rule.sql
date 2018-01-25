CREATE TABLE [rules].[Rule] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [TypeID]      INT            NOT NULL,
    [RuleDetails] NVARCHAR (MAX) NOT NULL,
    [BED]         DATETIME       NOT NULL,
    [EED]         DATETIME       NULL,
    [SourceID]    VARCHAR (255)  NULL,
    [IsDeleted]   BIT            NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_Rule] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Rule_RuleType] FOREIGN KEY ([TypeID]) REFERENCES [rules].[RuleType] ([ID])
);










GO
CREATE NONCLUSTERED INDEX [IX_Rule_RuleType]
    ON [rules].[Rule]([TypeID] ASC);

