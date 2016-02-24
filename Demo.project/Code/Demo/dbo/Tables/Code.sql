CREATE TABLE [dbo].[Code] (
    [ID]          BIGINT       NOT NULL,
    [Code]        VARCHAR (20) NOT NULL,
    [ZoneID]      BIGINT       NOT NULL,
    [BED]         DATETIME     NOT NULL,
    [EED]         DATETIME     NULL,
    [timestamp]   ROWVERSION   NULL,
    [CodeGroupID] INT          NULL,
    CONSTRAINT [PK_Code] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleCode_CodeGroup] FOREIGN KEY ([CodeGroupID]) REFERENCES [dbo].[CodeGroup] ([ID]),
    CONSTRAINT [FK_SaleCode_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [dbo].[Zone] ([ID])
);



