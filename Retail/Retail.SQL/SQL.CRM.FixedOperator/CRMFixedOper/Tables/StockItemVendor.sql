CREATE TABLE [CRMFixedOper].[StockItemVendor] (
    [ID]                UNIQUEIDENTIFIER NULL,
    [Name]              NVARCHAR (255)   NULL,
    [CreatedTime]       DATETIME         NULL,
    [CreatedBy]         INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [LastModifiedBy]    INT              NULL,
    [timestamp]         ROWVERSION       NULL,
    [MappedNIMVendorID] BIGINT           NULL
);

