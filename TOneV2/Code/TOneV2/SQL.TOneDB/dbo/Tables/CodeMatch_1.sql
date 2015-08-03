CREATE TABLE [dbo].[CodeMatch] (
    [Code]           VARCHAR (30) NOT NULL,
    [SupplierCodeID] BIGINT       NOT NULL,
    [SupplierZoneID] INT          NOT NULL,
    [SupplierID]     VARCHAR (5)  NULL,
    CONSTRAINT [FK_CodeMatch_CarrierAccount] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
);


GO
CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Supplier]
    ON [dbo].[CodeMatch]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Zone]
    ON [dbo].[CodeMatch]([SupplierZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Code]
    ON [dbo].[CodeMatch]([Code] ASC);

