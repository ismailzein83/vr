CREATE TABLE [dbo].[ZoneMatch] (
    [OurZoneID]      INT NOT NULL,
    [SupplierZoneID] INT NOT NULL,
    CONSTRAINT [PK_ZoneMatch] PRIMARY KEY CLUSTERED ([OurZoneID] ASC, [SupplierZoneID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ZoneMatch_SupplierZoneID]
    ON [dbo].[ZoneMatch]([SupplierZoneID] ASC);

