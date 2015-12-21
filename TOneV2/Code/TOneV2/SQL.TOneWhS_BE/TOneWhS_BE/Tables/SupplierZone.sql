CREATE TABLE [TOneWhS_BE].[SupplierZone] (
    [ID]         BIGINT         NOT NULL,
    [CountryID]  INT            NOT NULL,
    [Name]       NVARCHAR (255) NOT NULL,
    [SupplierID] INT            NOT NULL,
    [BED]        DATETIME       NOT NULL,
    [EED]        DATETIME       NULL,
    [timestamp]  ROWVERSION     NOT NULL,
    CONSTRAINT [PK_SupplierZone] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SupplierZone_CarrierAccount] FOREIGN KEY ([SupplierID]) REFERENCES [TOneWhS_BE].[CarrierAccount] ([ID])
);





