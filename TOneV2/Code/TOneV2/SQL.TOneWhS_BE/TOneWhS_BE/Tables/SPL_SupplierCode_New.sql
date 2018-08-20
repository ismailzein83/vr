CREATE TABLE [TOneWhS_BE].[SPL_SupplierCode_New] (
    [ID]                BIGINT       NOT NULL,
    [ProcessInstanceID] BIGINT       NOT NULL,
    [Code]              VARCHAR (20) NOT NULL,
    [ZoneID]            BIGINT       NOT NULL,
    [CodeGroupID]       INT          NULL,
    [BED]               DATETIME     NOT NULL,
    [EED]               DATETIME     NULL,
    [IsExcluded]        BIT          CONSTRAINT [DF_SPL_SupplierCode_New_IsExcluded] DEFAULT ((0)) NULL
);







