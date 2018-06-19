CREATE TABLE [TOneWhS_SPL].[ReceivedSupplierPricelist] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [SupplierID]          INT            NOT NULL,
    [FileID]              BIGINT         NULL,
    [ReceivedDate]        DATETIME       NOT NULL,
    [PricelistType]       INT            NULL,
    [Status]              INT            NOT NULL,
    [PricelistID]         INT            NULL,
    [ProcessInstanceId]   BIGINT         NULL,
    [StartProcessingDate] DATETIME       NULL,
    [ErrorDetails]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ReceivedSupplierPricelist] PRIMARY KEY CLUSTERED ([ID] ASC)
);

