CREATE TABLE [RetailBilling].[ContractServiceHistory] (
    [ID]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [ContractServiceID]       BIGINT           NULL,
    [ServiceTypeOptionID]     UNIQUEIDENTIFIER NULL,
    [BillingAccountID]        BIGINT           NULL,
    [StatusID]                UNIQUEIDENTIFIER NULL,
    [StatusReasonID]          UNIQUEIDENTIFIER NULL,
    [TechnologyID]            UNIQUEIDENTIFIER NULL,
    [SpecialNumberCategoryID] UNIQUEIDENTIFIER NULL,
    [SpeedInMbps]             DECIMAL (20, 4)  NULL,
    [SpeedType]               INT              NULL,
    [PackageLimitInGB]        INT              NULL,
    [ChargeableConditionID]   UNIQUEIDENTIFIER NULL,
    [BET]                     DATETIME         NULL,
    [EET]                     DATETIME         NULL,
    [CreatedTime]             DATETIME         NULL,
    [VoiceVolumeFixed]        INT              NULL,
    [VoiceVolumeMobile]       INT              NULL,
    [VoiceVolumePreferredNb]  INT              NULL,
    [Contract]                BIGINT           NULL,
    CONSTRAINT [PK__Contract__3214EC2701D345B0] PRIMARY KEY CLUSTERED ([ID] ASC)
);

