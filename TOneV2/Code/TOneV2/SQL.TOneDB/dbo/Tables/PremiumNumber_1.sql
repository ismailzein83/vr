CREATE TABLE [dbo].[PremiumNumber] (
    [PremiumNumberID]   INT            IDENTITY (1, 1) NOT NULL,
    [SupplierID]        VARCHAR (5)    NULL,
    [SaleZoneID]        INT            NULL,
    [CostZoneName]      NVARCHAR (255) NULL,
    [FromNumber]        VARCHAR (20)   NULL,
    [ToNumber]          VARCHAR (20)   NULL,
    [Rate]              DECIMAL (9, 5) NULL,
    [EffectiveDate]     SMALLDATETIME  NULL,
    [SaleZoneName]      NVARCHAR (255) NULL,
    [PremiumRateCardID] INT            NULL,
    [CodeRange]         VARCHAR (20)   NULL,
    [IsActive]          BIT            NULL,
    CONSTRAINT [PK_PremiumNumber] PRIMARY KEY CLUSTERED ([PremiumNumberID] ASC)
);

