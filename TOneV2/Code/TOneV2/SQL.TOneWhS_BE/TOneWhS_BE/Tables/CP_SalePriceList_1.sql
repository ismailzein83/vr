CREATE TABLE [TOneWhS_BE].[CP_SalePriceList] (
    [ID]                INT          NOT NULL,
    [OwnerType]         INT          NOT NULL,
    [OwnerID]           INT          NOT NULL,
    [CurrencyID]        INT          NOT NULL,
    [EffectiveOn]       DATETIME     NULL,
    [PriceListType]     TINYINT      NULL,
    [timestamp]         ROWVERSION   NULL,
    [SourceID]          VARCHAR (50) NULL,
    [ProcessInstanceID] BIGINT       NULL,
    [FileID]            BIGINT       NULL,
    [CreatedTime]       DATETIME     DEFAULT (getdate()) NULL,
    [IsSent]            BIT          DEFAULT ((0)) NOT NULL
);

