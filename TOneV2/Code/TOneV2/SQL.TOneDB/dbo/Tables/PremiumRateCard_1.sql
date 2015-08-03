CREATE TABLE [dbo].[PremiumRateCard] (
    [PremiumRateCardID] INT            IDENTITY (1, 1) NOT NULL,
    [SaleZoneID]        INT            NULL,
    [SubZoneName]       NVARCHAR (250) NULL,
    [Payout]            DECIMAL (9, 5) NULL,
    [TestNumber]        NVARCHAR (20)  NULL,
    [RangeLength]       INT            NULL,
    [CurrencyID]        VARCHAR (3)    NULL,
    [Range]             VARCHAR (20)   NOT NULL,
    [ParentCode]        VARCHAR (50)   NULL,
    [IsActive]          BIT            NULL,
    CONSTRAINT [PK_PremiumRateCard] PRIMARY KEY CLUSTERED ([PremiumRateCardID] ASC)
);

