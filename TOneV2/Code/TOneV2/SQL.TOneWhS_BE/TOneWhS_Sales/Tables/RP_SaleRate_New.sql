CREATE TABLE [TOneWhS_Sales].[RP_SaleRate_New] (
    [ID]                BIGINT         NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [ZoneID]            BIGINT         NOT NULL,
    [RateTypeID]        INT            NULL,
    [NormalRate]        DECIMAL (9, 5) NOT NULL,
    [OtherRates]        NVARCHAR (MAX) NULL,
    [CurrencyID]        INT            NOT NULL,
    [BED]               DATETIME       NOT NULL,
    [EED]               DATETIME       NULL
);



