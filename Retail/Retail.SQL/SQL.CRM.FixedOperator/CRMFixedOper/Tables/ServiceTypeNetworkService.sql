CREATE TABLE [CRMFixedOper].[ServiceTypeNetworkService] (
    [ID]                 UNIQUEIDENTIFIER NOT NULL,
    [ServiceType]        UNIQUEIDENTIFIER NULL,
    [NetworkServiceType] UNIQUEIDENTIFIER NULL,
    [CreatedTime]        DATETIME         NULL,
    [CreatedBy]          INT              NULL,
    [LastModifiedTime]   DATETIME         NULL,
    [LastModifiedBy]     INT              NULL,
    [timestamp]          ROWVERSION       NULL,
    [ServiceTypeOption]  UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

