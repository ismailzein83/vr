CREATE TABLE [dbo].[PricelistEmailQueueMetaData] (
    [ID]                     INT            IDENTITY (1, 1) NOT NULL,
    [MessageUID]             NVARCHAR (100) NOT NULL,
    [Status]                 INT            NOT NULL,
    [DateTimeSentBySupplier] DATETIME       NOT NULL,
    [DateTimeProcessed]      DATETIME       NULL,
    [PricelistID]            INT            NULL,
    [SupplierID]             VARCHAR (5)    NOT NULL,
    [EffectivePricelistID]   INT            NULL,
    [PriceListType]          NVARCHAR (100) NULL,
    CONSTRAINT [PK_PricelistEmailQueueMetaData] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_QueueDateTime]
    ON [dbo].[PricelistEmailQueueMetaData]([DateTimeSentBySupplier] ASC, [Status] ASC)
    INCLUDE([SupplierID]);

