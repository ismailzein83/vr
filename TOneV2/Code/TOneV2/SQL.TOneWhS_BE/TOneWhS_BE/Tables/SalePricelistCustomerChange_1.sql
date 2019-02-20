CREATE TABLE [TOneWhS_BE].[SalePricelistCustomerChange] (
    [BatchID]          INT      NULL,
    [PricelistID]      INT      NULL,
    [CountryID]        INT      NULL,
    [CustomerID]       INT      NULL,
    [LastModifiedTime] DATETIME CONSTRAINT [DF_SalePricelistCustomerChange_LastModifiedTime] DEFAULT (getdate()) NULL,
    [CreatedTime]      DATETIME CONSTRAINT [DF_SalePricelistCustomerChange_CreatedTime] DEFAULT (getdate()) NULL
);








GO
CREATE CLUSTERED INDEX [IX_SalePricelistCustomerChange_BatchID]
    ON [TOneWhS_BE].[SalePricelistCustomerChange]([BatchID] ASC);

