CREATE TABLE [RetailBilling].[ContractSubType_VPN] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [LinkTypeID]       UNIQUEIDENTIFIER NULL,
    [Speed]            INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

