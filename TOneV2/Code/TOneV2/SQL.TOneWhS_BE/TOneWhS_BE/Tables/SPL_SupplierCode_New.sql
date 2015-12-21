CREATE TABLE [TOneWhS_BE].[SPL_SupplierCode_New] (
    [ID]          BIGINT       NOT NULL,
    [PriceListID] INT          NOT NULL,
    [Code]        VARCHAR (20) NOT NULL,
    [ZoneID]      BIGINT       NOT NULL,
    [CodeGroupID] INT          NULL,
    [BED]         DATETIME     NOT NULL,
    [EED]         DATETIME     NULL
);

