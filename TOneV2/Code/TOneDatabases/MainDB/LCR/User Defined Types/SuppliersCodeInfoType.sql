CREATE TYPE [LCR].[SuppliersCodeInfoType] AS TABLE (
    [SupplierID]      VARCHAR (5) NOT NULL,
    [HasUpdatedCodes] BIT         NOT NULL,
    PRIMARY KEY CLUSTERED ([SupplierID] ASC));

