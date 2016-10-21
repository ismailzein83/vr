CREATE TABLE [TOneWhS_Sales].[RP_SaleRate_New] (
    [ID]                BIGINT          NULL,
    [ProcessInstanceID] BIGINT          NOT NULL,
    [ZoneID]            BIGINT          NOT NULL,
    [RateTypeID]        INT             NULL,
    [Rate]              DECIMAL (20, 8) NOT NULL,
    [CurrencyID]        INT             NULL,
    [BED]               DATETIME        NOT NULL,
    [EED]               DATETIME        NULL
);







