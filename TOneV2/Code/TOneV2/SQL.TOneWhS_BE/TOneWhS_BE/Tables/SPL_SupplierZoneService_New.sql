CREATE TABLE [TOneWhS_BE].[SPL_SupplierZoneService_New] (
    [ID]                INT            NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [ZoneID]            BIGINT         NOT NULL,
    [ZoneServices]      NVARCHAR (MAX) NOT NULL,
    [BED]               DATETIME       NOT NULL,
    [EED]               DATETIME       NULL
);

