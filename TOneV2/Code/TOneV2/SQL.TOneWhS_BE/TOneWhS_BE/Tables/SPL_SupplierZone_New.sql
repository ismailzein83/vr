CREATE TABLE [TOneWhS_BE].[SPL_SupplierZone_New] (
    [ID]                INT            NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [CountryID]         INT            NOT NULL,
    [Name]              NVARCHAR (255) NOT NULL,
    [SupplierID]        INT            NOT NULL,
    [BED]               DATETIME       NOT NULL,
    [EED]               DATETIME       NULL,
    [IsExcluded]        BIT            CONSTRAINT [DF_SPL_SupplierZone_New_IsExcluded] DEFAULT ((0)) NULL
);







