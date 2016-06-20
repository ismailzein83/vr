﻿CREATE TABLE [TOneWhS_BE].[SaleCode] (
    [ID]          BIGINT       NOT NULL,
    [Code]        VARCHAR (20) NOT NULL,
    [ZoneID]      BIGINT       NOT NULL,
    [CodeGroupID] INT          NULL,
    [BED]         DATETIME     NOT NULL,
    [EED]         DATETIME     NULL,
    [timestamp]   ROWVERSION   NULL,
    [SourceID]    VARCHAR (50) NULL,
    CONSTRAINT [PK_SaleCode] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SaleCode_CodeGroup] FOREIGN KEY ([CodeGroupID]) REFERENCES [TOneWhS_BE].[CodeGroup] ([ID]),
    CONSTRAINT [FK_SaleCode_SaleZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SaleZone] ([ID])
);

