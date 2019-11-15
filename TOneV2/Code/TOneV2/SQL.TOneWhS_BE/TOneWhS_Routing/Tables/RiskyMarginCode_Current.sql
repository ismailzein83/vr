CREATE TABLE [TOneWhS_Routing].[RiskyMarginCode_Current] (
    [Code]                  VARCHAR (20) NOT NULL,
    [CustomerRouteMarginID] BIGINT       NOT NULL,
    [CreatedTime]           DATETIME     NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_RiskyMarginCode_CustomerRouteMarginID_93e1d7d2d74e4e4dbbeaa449f896325a]
    ON [TOneWhS_Routing].[RiskyMarginCode_Current]([CustomerRouteMarginID] ASC);

