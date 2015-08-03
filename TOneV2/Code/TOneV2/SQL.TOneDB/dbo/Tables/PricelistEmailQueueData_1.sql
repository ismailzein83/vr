CREATE TABLE [dbo].[PricelistEmailQueueData] (
    [MetaDataItemID]     INT           NOT NULL,
    [AttachmentData]     IMAGE         NOT NULL,
    [PricelistImportLog] IMAGE         NULL,
    [FileName]           VARCHAR (150) NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_QueueData]
    ON [dbo].[PricelistEmailQueueData]([MetaDataItemID] ASC);

