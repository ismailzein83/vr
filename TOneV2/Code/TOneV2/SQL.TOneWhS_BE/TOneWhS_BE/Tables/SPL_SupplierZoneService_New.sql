CREATE TABLE [TOneWhS_BE].[SPL_SupplierZoneService_New] (
    [ID]                INT            NULL,
    [SupplierID]        INT            NOT NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [ZoneID]            BIGINT         NOT NULL,
    [ZoneServices]      NVARCHAR (MAX) NOT NULL,
    [BED]               DATETIME       NOT NULL,
    [EED]               DATETIME       NULL,
    [IsExcluded]        BIT            CONSTRAINT [DF_SPL_SupplierZoneService_New_IsExcluded] DEFAULT ((0)) NULL
);







