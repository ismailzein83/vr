CREATE TABLE [RetailBilling].[Contract] (
    [ID]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [ContractTypeID]          UNIQUEIDENTIFIER NULL,
    [CustomerID]              BIGINT           NULL,
    [ResourceName]            NVARCHAR (255)   NULL,
    [StatusID]                UNIQUEIDENTIFIER NULL,
    [RatePlanID]              INT              NULL,
    [CreatedTime]             DATETIME         NULL,
    [CreatedBy]               INT              NULL,
    [LastModifiedTime]        DATETIME         NULL,
    [LastModifiedBy]          INT              NULL,
    [BillingAccountID]        BIGINT           NULL,
    [ActivationDate]          DATETIME         NULL,
    [SuspensionDate]          DATETIME         NULL,
    [CustomFields]            NVARCHAR (MAX)   NULL,
    [TechnologyID]            UNIQUEIDENTIFIER NULL,
    [HasTelephony]            BIT              NULL,
    [HasInternet]             BIT              NULL,
    [SpecialNumberCategoryID] UNIQUEIDENTIFIER NULL,
    [SpeedInMbps]             DECIMAL (20, 4)  NULL,
    [SpeedType]               INT              NULL,
    [PackageLimitInGB]        INT              NULL,
    [NbOfLinks]               INT              NULL,
    [ActivationTime]          DATETIME         NULL,
    [DeactivationTime]        DATETIME         NULL,
    [DunningStatusID]         UNIQUEIDENTIFIER NULL,
    [StatusReasonID]          UNIQUEIDENTIFIER NULL,
    [NIMPathID]               BIGINT           NULL,
    CONSTRAINT [PK__Contract__3214EC2707C12930] PRIMARY KEY CLUSTERED ([ID] ASC)
);







