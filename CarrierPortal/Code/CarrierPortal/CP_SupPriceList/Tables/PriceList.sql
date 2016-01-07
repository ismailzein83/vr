CREATE TABLE [CP_SupPriceList].[PriceList] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserID]            INT            NOT NULL,
    [FileID]            BIGINT         NOT NULL,
    [PriceListType]     INT            NOT NULL,
    [Status]            INT            NOT NULL,
    [Result]            INT            CONSTRAINT [DF_PriceList_Result] DEFAULT ((0)) NOT NULL,
    [UploadInformation] NVARCHAR (MAX) NULL,
    [PriceListProgress] NVARCHAR (MAX) NULL,
    [CreatedTime]       DATETIME       NOT NULL,
    [EffectiveOnDate]   DATETIME       NOT NULL,
    [timestamp]         ROWVERSION     NULL,
    CONSTRAINT [PK_PriceList] PRIMARY KEY CLUSTERED ([ID] ASC)
);

