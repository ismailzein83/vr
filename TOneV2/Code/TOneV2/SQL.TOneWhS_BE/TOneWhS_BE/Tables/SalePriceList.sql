CREATE TABLE [TOneWhS_BE].[SalePriceList] (
    [ID]                INT            NOT NULL,
    [OwnerType]         INT            NOT NULL,
    [OwnerID]           INT            NOT NULL,
    [CurrencyID]        INT            NOT NULL,
    [EffectiveOn]       DATETIME       NULL,
    [PriceListType]     TINYINT        NULL,
    [timestamp]         ROWVERSION     NULL,
    [SourceID]          VARCHAR (50)   NULL,
    [ProcessInstanceID] BIGINT         NULL,
    [FileID]            BIGINT         NULL,
    [IsSent]            BIT            CONSTRAINT [DF_SalePriceList_IsSent] DEFAULT ((0)) NULL,
    [CreatedTime]       DATETIME       CONSTRAINT [cce5d9f5-0099-4b32-9375-2cb3bc44b990] DEFAULT (getdate()) NULL,
    [UserID]            INT            NULL,
    [Description]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_SalePriceList] PRIMARY KEY CLUSTERED ([ID] ASC)
);
























GO
CREATE NONCLUSTERED INDEX [IX_SalePriceList_timestamp]
    ON [TOneWhS_BE].[SalePriceList]([timestamp] DESC);

