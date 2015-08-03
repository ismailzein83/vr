CREATE TABLE [dbo].[PriceList] (
    [PriceListID]        INT            IDENTITY (1, 1) NOT NULL,
    [SupplierID]         VARCHAR (5)    NOT NULL,
    [CustomerID]         VARCHAR (5)    NOT NULL,
    [Description]        NVARCHAR (MAX) NULL,
    [CurrencyID]         VARCHAR (3)    NOT NULL,
    [BeginEffectiveDate] SMALLDATETIME  NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [IsEffective]        AS             (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
    [SourceFileName]     NVARCHAR (200) NULL,
    [UserID]             INT            NULL,
    [timestamp]          ROWVERSION     NULL,
    [IsSent]             CHAR (1)       NULL,
    CONSTRAINT [PK_PriceList] PRIMARY KEY CLUSTERED ([PriceListID] ASC),
    CONSTRAINT [FK_PriceList_Customer] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID]),
    CONSTRAINT [FK_PriceList_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
);


GO
CREATE NONCLUSTERED INDEX [IX_PriceList_Dates]
    ON [dbo].[PriceList]([BeginEffectiveDate] DESC, [EndEffectiveDate] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_PriceList_Customer]
    ON [dbo].[PriceList]([CustomerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PriceList_Supplier]
    ON [dbo].[PriceList]([SupplierID] ASC);

