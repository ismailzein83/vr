CREATE TABLE [TOneWhS_BE].[SalePricelistCustomerChange_New] (
    [BatchID]     INT NOT NULL,
    [PricelistID] INT NOT NULL,
    [CountryID]   INT NOT NULL,
    [CustomerID]  INT NOT NULL
);




GO
CREATE CLUSTERED INDEX [IX_SalePricelistCustomerChange_New_BatchID]
    ON [TOneWhS_BE].[SalePricelistCustomerChange_New]([BatchID] ASC);

