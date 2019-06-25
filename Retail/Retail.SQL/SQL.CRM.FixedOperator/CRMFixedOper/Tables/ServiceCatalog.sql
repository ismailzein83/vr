CREATE TABLE [CRMFixedOper].[ServiceCatalog] (
    [ID]                             UNIQUEIDENTIFIER NOT NULL,
    [ServiceID]                      UNIQUEIDENTIFIER NOT NULL,
    [ContractTypeID]                 UNIQUEIDENTIFIER NULL,
    [CreatedTime]                    DATETIME         NULL,
    [CreatedBy]                      INT              NULL,
    [LastModifiedTime]               DATETIME         NULL,
    [LastModifiedBy]                 INT              NULL,
    [timestamp]                      ROWVERSION       NULL,
    [EligibleTechnologies]           NVARCHAR (MAX)   NULL,
    [EligibleToAllTechnologies]      BIT              NULL,
    [RestrictedServiceGroups]        NVARCHAR (MAX)   NULL,
    [EligibleToSpecificTechnologies] BIT              NULL,
    [ServiceAvailability]            INT              NULL,
    [SelectedByDefault]              BIT              NULL,
    [RequiredOption]                 INT              NULL,
    CONSTRAINT [PK_ServiceCatalog] PRIMARY KEY CLUSTERED ([ID] ASC)
);

