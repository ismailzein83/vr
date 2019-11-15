CREATE TABLE [RetailBilling].[ContractHistory] (
    [ID]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [ContractID]              BIGINT           NULL,
    [BillingAccountID]        BIGINT           NULL,
    [RatePlanID]              INT              NULL,
    [MainResourceName]        NVARCHAR (255)   NULL,
    [StatusID]                UNIQUEIDENTIFIER NULL,
    [StatusReasonID]          UNIQUEIDENTIFIER NULL,
    [TechnologyID]            UNIQUEIDENTIFIER NULL,
    [HasTelephony]            BIT              NULL,
    [HasInternet]             BIT              NULL,
    [SpecialNumberCategoryID] UNIQUEIDENTIFIER NULL,
    [SpeedInMbps]             DECIMAL (20, 4)  NULL,
    [SpeedType]               INT              NULL,
    [PackageLimitInGB]        INT              NULL,
    [NbOfLinks]               INT              NULL,
    [CreatedTime]             DATETIME         NULL,
    [BET]                     DATETIME         NULL,
    [EET]                     DATETIME         NULL,
    [NIMPathID]               BIGINT           NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

