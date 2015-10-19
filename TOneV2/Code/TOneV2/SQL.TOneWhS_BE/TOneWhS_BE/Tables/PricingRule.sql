CREATE TABLE [TOneWhS_BE].[PricingRule] (
    [ID]           INT            IDENTITY (1, 1) NOT NULL,
    [Criteria]     NVARCHAR (MAX) NOT NULL,
    [TypeConfigID] INT            NOT NULL,
    [RuleSettings] NVARCHAR (MAX) NULL,
    [Description]  NVARCHAR (MAX) NULL,
    [BED]          DATETIME       NOT NULL,
    [EED]          NCHAR (10)     NULL,
    CONSTRAINT [PK_PricingRule] PRIMARY KEY CLUSTERED ([ID] ASC)
);

