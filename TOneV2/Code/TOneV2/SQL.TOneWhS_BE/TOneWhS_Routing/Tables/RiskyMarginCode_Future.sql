CREATE TABLE [TOneWhS_Routing].[RiskyMarginCode_Future] (
    [Code]                  VARCHAR (20) NOT NULL,
    [CustomerRouteMarginID] BIGINT       NOT NULL,
    [CreatedTime]           DATETIME     NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_RiskyMarginCode_CustomerRouteMarginID_0ef5102d599c499eb4129547eeb303af]
    ON [TOneWhS_Routing].[RiskyMarginCode_Future]([CustomerRouteMarginID] ASC);

