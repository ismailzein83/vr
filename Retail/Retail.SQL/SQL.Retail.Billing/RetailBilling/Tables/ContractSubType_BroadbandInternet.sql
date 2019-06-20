CREATE TABLE [RetailBilling].[ContractSubType_BroadbandInternet] (
    [ID]                UNIQUEIDENTIFIER NOT NULL,
    [DataServiceTypeID] UNIQUEIDENTIFIER NULL,
    [SpeedType]         INT              NULL,
    [Speed]             INT              NULL,
    [PackageLimit]      INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [timestamp]         ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

