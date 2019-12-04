CREATE TABLE [CRMFixedOper].[ContractServiceTypeOption] (
    [ID]                         UNIQUEIDENTIFIER NOT NULL,
    [Name]                       NVARCHAR (255)   NULL,
    [ContractServiceTypeID]      UNIQUEIDENTIFIER NULL,
    [RestrictToTechnologies]     NVARCHAR (MAX)   NULL,
    [NeedMOTDecision]            BIT              NULL,
    [SpeedInMbps]                DECIMAL (20, 4)  NULL,
    [SpeedType]                  INT              NULL,
    [QuotaInGB]                  INT              NULL,
    [FromSpeedInMbps]            DECIMAL (20, 4)  NULL,
    [ToSpeedInMbps]              DECIMAL (20, 4)  NULL,
    [NIMNumberCategoryID]        UNIQUEIDENTIFIER NULL,
    [OLDNIMNumberCategoryID]     INT              NULL,
    [NumberOfMinutesFixed]       INT              NULL,
    [NumberOfMinutesMobile]      INT              NULL,
    [NumberOfMinutesPreferredNb] INT              NULL,
    [BundleValidityInDays]       INT              NULL,
    [DecreeID]                   INT              NULL,
    [CreatedTime]                DATETIME         NULL,
    [CreatedBy]                  INT              NULL,
    [LastModifiedTime]           DATETIME         NULL,
    [LastModifiedBy]             INT              NULL,
    [timestamp]                  ROWVERSION       NULL,
    [RestrictToTechnologyID]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__Contract__3214EC2729E1370A] PRIMARY KEY CLUSTERED ([ID] ASC)
);



