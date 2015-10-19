CREATE TABLE [rules].[Rule] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [TypeID]      INT            NOT NULL,
    [RuleDetails] NVARCHAR (MAX) NOT NULL,
    [BED]         DATETIME       NULL,
    [EED]         DATETIME       NOT NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_Rule] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Rule_RuleType]
    ON [rules].[Rule]([TypeID] ASC);

