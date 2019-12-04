CREATE TABLE [RetailBilling].[ContractService] (
    [ID]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [ContractID]              BIGINT           NULL,
    [ServiceTypeID]           UNIQUEIDENTIFIER NULL,
    [ServiceTypeOptionID]     UNIQUEIDENTIFIER NULL,
    [ServiceID]               UNIQUEIDENTIFIER NULL,
    [StatusID]                UNIQUEIDENTIFIER NULL,
    [BillingAccountID]        BIGINT           NULL,
    [ChargeableConditionID]   UNIQUEIDENTIFIER NULL,
    [ActivationDate]          DATETIME         NULL,
    [CreatedTime]             DATETIME         NULL,
    [CreatedBy]               INT              NULL,
    [LastModifiedTime]        DATETIME         NULL,
    [LastModifiedBy]          INT              NULL,
    [SuspensionDate]          DATETIME         NULL,
    [TechnologyID]            UNIQUEIDENTIFIER NULL,
    [SpecialNumberCategoryID] UNIQUEIDENTIFIER NULL,
    [SpeedInMbps]             DECIMAL (20, 4)  NULL,
    [SpeedType]               INT              NULL,
    [PackageLimitInGB]        INT              NULL,
    [ActivationTime]          DATETIME         NULL,
    [DeactivationTime]        DATETIME         NULL,
    [VoiceVolumeFixed]        INT              NULL,
    [VoiceVolumeMobile]       INT              NULL,
    [VoiceVolumePreferredNb]  INT              NULL,
    [StatusReasonID]          UNIQUEIDENTIFIER NULL,
    [NIMPathID]               BIGINT           NULL,
    CONSTRAINT [PK__Contract__3214EC270B91BA14] PRIMARY KEY CLUSTERED ([ID] ASC)
);







