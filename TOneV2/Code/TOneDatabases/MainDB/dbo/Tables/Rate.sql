CREATE TABLE [dbo].[Rate] (
    [RateID]             BIGINT         IDENTITY (1, 1) NOT NULL,
    [PriceListID]        INT            NULL,
    [ZoneID]             INT            NULL,
    [Rate]               DECIMAL (9, 5) NULL,
    [OffPeakRate]        DECIMAL (9, 5) NULL,
    [WeekendRate]        DECIMAL (9, 5) NULL,
    [Change]             SMALLINT       NULL,
    [ServicesFlag]       SMALLINT       NULL,
    [BeginEffectiveDate] SMALLDATETIME  NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [IsEffective]        AS             (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
    [UserID]             INT            NULL,
    [timestamp]          ROWVERSION     NULL,
    [Notes]              NVARCHAR (255) NULL,
    CONSTRAINT [PK_Rate] PRIMARY KEY CLUSTERED ([RateID] ASC),
    CONSTRAINT [FK_Rate_PriceList] FOREIGN KEY ([PriceListID]) REFERENCES [dbo].[PriceList] ([PriceListID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Rate_Dates]
    ON [dbo].[Rate]([BeginEffectiveDate] DESC, [EndEffectiveDate] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_Rate_Zone]
    ON [dbo].[Rate]([ZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Rate_Pricelist]
    ON [dbo].[Rate]([PriceListID] ASC);

