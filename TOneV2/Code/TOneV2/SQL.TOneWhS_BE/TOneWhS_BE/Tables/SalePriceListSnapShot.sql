CREATE TABLE [TOneWhS_BE].[SalePriceListSnapShot] (
    [PriceListID]      INT            NOT NULL,
    [SnapShotDetail]   NVARCHAR (MAX) NULL,
    [Timestamp]        ROWVERSION     NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_SalePriceListSnapShot_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME       CONSTRAINT [DF_SalePriceListSnapShot_LastModifiedTime] DEFAULT (getdate()) NULL
);








GO
CREATE CLUSTERED INDEX [IX_SalePriceListSnapShot_PriceListID]
    ON [TOneWhS_BE].[SalePriceListSnapShot]([PriceListID] ASC);

