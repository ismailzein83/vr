CREATE TABLE [TOneWhS_Sales].[RP_RatePlanPreview_Summary] (
    [ProcessInstanceID]                     BIGINT         NOT NULL,
    [NumberOfNewRates]                      INT            NOT NULL,
    [NumberOfIncreasedRates]                INT            NOT NULL,
    [NumberOfDecreasedRates]                INT            NOT NULL,
    [NumberOfClosedRates]                   INT            NOT NULL,
    [NameOfNewDefaultRoutingProduct]        NVARCHAR (255) NULL,
    [NameOfClosedDefaultRoutingProduct]     NVARCHAR (255) NULL,
    [NumberOfNewSaleZoneRoutingProducts]    INT            NOT NULL,
    [NumberOfClosedSaleZoneRoutingProducts] INT            NOT NULL,
    [NewDefaultServices]                    NVARCHAR (MAX) NULL,
    [ClosedDefaultServiceEffectiveOn]       DATETIME       NULL,
    [NumberOfNewSaleZoneServices]           INT            NOT NULL,
    [NumberOfClosedSaleZoneServices]        INT            NOT NULL,
    [NumberOfChangedCountries]              INT            NOT NULL,
    [NumberOfNewCountries]                  INT            NULL,
    [NumberOfNewOtherRates]                 INT            NOT NULL,
    [NumberOfIncreasedOtherRates]           INT            NOT NULL,
    [NumberOfDecreasedOtherRates]           INT            NOT NULL
);












GO
CREATE CLUSTERED INDEX [IX_RP_RatePlanPreview_Summary_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_RatePlanPreview_Summary]([ProcessInstanceID] ASC);

