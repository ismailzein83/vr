CREATE TABLE [TOneWhS_BE].[SPL_SupplierRate_New] (
    [ID]                BIGINT          NULL,
    [ProcessInstanceID] BIGINT          NOT NULL,
    [ZoneID]            BIGINT          NOT NULL,
    [CurrencyID]        INT             NULL,
    [NormalRate]        DECIMAL (20, 8) NOT NULL,
    [OtherRates]        VARCHAR (MAX)   NULL,
    [BED]               DATETIME        NOT NULL,
    [EED]               DATETIME        NULL
);





