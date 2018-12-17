CREATE TABLE [TOneWhS_BE].[SupplierZone] (
    [ID]               BIGINT         NOT NULL,
    [CountryID]        INT            NOT NULL,
    [Name]             NVARCHAR (255) NOT NULL,
    [SupplierID]       INT            NOT NULL,
    [BED]              DATETIME       NOT NULL,
    [EED]              DATETIME       NULL,
    [timestamp]        ROWVERSION     NOT NULL,
    [SourceID]         VARCHAR (50)   NULL,
    [LastModifiedTime] DATETIME       NULL,
    CONSTRAINT [PK_SupplierZone] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SupplierZone_CarrierAccount] FOREIGN KEY ([SupplierID]) REFERENCES [TOneWhS_BE].[CarrierAccount] ([ID])
);












GO
CREATE NONCLUSTERED INDEX [IX_SupplierZone_timestamp]
    ON [TOneWhS_BE].[SupplierZone]([timestamp] DESC);

