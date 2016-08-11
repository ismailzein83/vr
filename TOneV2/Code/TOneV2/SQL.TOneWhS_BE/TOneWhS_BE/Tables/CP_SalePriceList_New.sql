CREATE TABLE [TOneWhS_BE].[CP_SalePriceList_New] (
    [ID]                BIGINT         NOT NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [OwnerType]         INT            NOT NULL,
    [OwnerID]           INT            NOT NULL,
    [CurrencyID]        NVARCHAR (255) NOT NULL,
    [EffectiveOn]       DATETIME       NOT NULL
);

