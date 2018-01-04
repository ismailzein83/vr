CREATE TABLE [TOneWhS_BE].[SalePriceList_New] (
    [ID]                     INT            NOT NULL,
    [OwnerType]              INT            NOT NULL,
    [OwnerID]                INT            NOT NULL,
    [CurrencyID]             INT            NOT NULL,
    [EffectiveOn]            DATETIME       NULL,
    [PriceListType]          TINYINT        NULL,
    [timestamp]              ROWVERSION     NULL,
    [SourceID]               VARCHAR (50)   NULL,
    [ProcessInstanceID]      BIGINT         NULL,
    [FileID]                 BIGINT         NULL,
    [CreatedTime]            DATETIME       CONSTRAINT [DF__CP_SalePr__Creat__22751F6C] DEFAULT (getdate()) NULL,
    [IsSent]                 BIT            CONSTRAINT [DF__CP_SalePr__IsSen__236943A5] DEFAULT ((0)) NOT NULL,
    [UserID]                 INT            NULL,
    [Description]            NVARCHAR (MAX) NULL,
    [PricelistStateBackupID] BIGINT         NULL
);










GO
CREATE CLUSTERED INDEX [IX_SalePriceList_New_ProcessInstanceID]
    ON [TOneWhS_BE].[SalePriceList_New]([ProcessInstanceID] ASC);

