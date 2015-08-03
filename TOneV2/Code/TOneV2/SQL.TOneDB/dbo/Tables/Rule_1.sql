CREATE TABLE [dbo].[Rule] (
    [ID]           INT         IDENTITY (1, 1) NOT NULL,
    [ParentRuleID] INT         NULL,
    [ServicesFlag] SMALLINT    NOT NULL,
    [OwnerMCTID]   INT         NOT NULL,
    [ZoneID]       INT         NULL,
    [Type]         INT         NOT NULL,
    [RateFrom]     FLOAT (53)  NULL,
    [RateTo]       FLOAT (53)  NULL,
    [MarginFrom]   FLOAT (53)  NULL,
    [MarginTo]     FLOAT (53)  NULL,
    [IsPercentage] BIT         NULL,
    [Target]       INT         NULL,
    [SupplierID]   VARCHAR (5) NULL,
    CONSTRAINT [PK_Rule] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Rule_MCT] FOREIGN KEY ([OwnerMCTID]) REFERENCES [dbo].[MCT] ([ID])
);

