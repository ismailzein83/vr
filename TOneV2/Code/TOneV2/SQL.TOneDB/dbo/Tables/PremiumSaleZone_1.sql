CREATE TABLE [dbo].[PremiumSaleZone] (
    [PremiumSaleZoneID] INT            IDENTITY (1, 1) NOT NULL,
    [SaleZoneID]        INT            NULL,
    [SaleZoneName]      NVARCHAR (255) NULL,
    [EffectiveCodes]    VARCHAR (MAX)  NULL,
    [IsActive]          BIT            NULL,
    CONSTRAINT [PK_PremiumSaleZone] PRIMARY KEY CLUSTERED ([PremiumSaleZoneID] ASC)
);

