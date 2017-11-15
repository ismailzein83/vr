CREATE TABLE [TOneWhS_BE].[SalePriceListSnapShot] (
    [PriceListID]    INT            NOT NULL,
    [SnapShotDetail] NVARCHAR (MAX) NULL,
    [Timestamp]      ROWVERSION     NULL
);






GO
CREATE CLUSTERED INDEX [IX_SalePriceListSnapShot_PriceListID]
    ON [TOneWhS_BE].[SalePriceListSnapShot]([PriceListID] ASC);

