CREATE TABLE [TOneWhS_BE].[SPL_SupplierRate_Changed] (
    [ID]                BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [EED]               DATETIME NULL,
    [IsExcluded]        BIT      CONSTRAINT [DF_SPL_SupplierRate_Changed_IsExcluded] DEFAULT ((0)) NULL
);







