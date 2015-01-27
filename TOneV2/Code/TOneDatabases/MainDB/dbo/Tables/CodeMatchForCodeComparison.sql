CREATE TABLE [dbo].[CodeMatchForCodeComparison] (
    [Code]           VARCHAR (15) NOT NULL,
    [MatchingCode]   VARCHAR (25) NOT NULL,
    [SupplierCodeID] BIGINT       NOT NULL,
    [SupplierZoneID] INT          NOT NULL,
    [SupplierID]     VARCHAR (5)  NULL
);


GO
CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Supplier]
    ON [dbo].[CodeMatchForCodeComparison]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Zone]
    ON [dbo].[CodeMatchForCodeComparison]([SupplierZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Code]
    ON [dbo].[CodeMatchForCodeComparison]([Code] ASC);

