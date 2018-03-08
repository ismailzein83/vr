CREATE TABLE [TOneWhS_BE].[ANumberSaleCode] (
    [ID]                  BIGINT       NOT NULL,
    [ANumberGroupID]      INT          NOT NULL,
    [SellingNumberPlanID] INT          NOT NULL,
    [Code]                VARCHAR (20) NOT NULL,
    [BED]                 DATETIME     NOT NULL,
    [EED]                 DATETIME     NULL,
    [timestamp]           ROWVERSION   NULL,
    [CreatedTime]         DATETIME     CONSTRAINT [DF_ANumberSaleCode_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ANumberSaleCode] PRIMARY KEY CLUSTERED ([ID] ASC)
);

