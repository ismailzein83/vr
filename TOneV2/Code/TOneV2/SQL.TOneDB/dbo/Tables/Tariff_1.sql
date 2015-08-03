CREATE TABLE [dbo].[Tariff] (
    [TariffID]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [ZoneID]             INT             NULL,
    [CustomerID]         VARCHAR (10)    NOT NULL,
    [SupplierID]         VARCHAR (10)    NOT NULL,
    [CallFee]            DECIMAL (13, 6) CONSTRAINT [DF_Tariff_CallFee] DEFAULT ((0)) NOT NULL,
    [FirstPeriodRate]    DECIMAL (13, 6) NULL,
    [FirstPeriod]        TINYINT         NULL,
    [RepeatFirstPeriod]  CHAR (1)        NULL,
    [FractionUnit]       TINYINT         NULL,
    [BeginEffectiveDate] SMALLDATETIME   NULL,
    [EndEffectiveDate]   SMALLDATETIME   NULL,
    [IsEffective]        AS              (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
    [UserID]             INT             NULL,
    [timestamp]          ROWVERSION      NULL,
    CONSTRAINT [PK_Tariff] PRIMARY KEY CLUSTERED ([TariffID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Tariff_Supplier]
    ON [dbo].[Tariff]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tariff_Customer]
    ON [dbo].[Tariff]([CustomerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Tariff_Dates]
    ON [dbo].[Tariff]([BeginEffectiveDate] DESC, [EndEffectiveDate] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_Tariff_Zone]
    ON [dbo].[Tariff]([ZoneID] ASC);

