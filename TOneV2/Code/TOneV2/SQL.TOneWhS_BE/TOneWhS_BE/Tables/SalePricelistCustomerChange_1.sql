CREATE TABLE [TOneWhS_BE].[SalePricelistCustomerChange] (
    [BatchID]     INT NULL,
    [PricelistID] INT NULL,
    [CountryID]   INT NULL,
    [CustomerID]  INT NULL
);






GO
CREATE CLUSTERED INDEX [IX_SalePricelistCustomerChange_BatchID]
    ON [TOneWhS_BE].[SalePricelistCustomerChange]([BatchID] ASC);

