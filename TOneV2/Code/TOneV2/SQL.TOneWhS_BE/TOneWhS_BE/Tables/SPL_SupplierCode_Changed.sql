CREATE TABLE [TOneWhS_BE].[SPL_SupplierCode_Changed] (
    [ID]                BIGINT   NOT NULL,
    [ProcessInstanceID] BIGINT   NOT NULL,
    [EED]               DATETIME NULL,
    [IsExcluded]        BIT      CONSTRAINT [DF_SPL_SupplierCode_Changed_IsExcluded] DEFAULT ((0)) NULL
);







