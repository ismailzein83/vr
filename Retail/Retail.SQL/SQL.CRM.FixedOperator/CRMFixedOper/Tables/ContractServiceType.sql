CREATE TABLE [CRMFixedOper].[ContractServiceType] (
    [ID]                              UNIQUEIDENTIFIER NOT NULL,
    [Name]                            NVARCHAR (255)   NULL,
    [ContractTypeID]                  UNIQUEIDENTIFIER NULL,
    [ServiceCategoryID]               UNIQUEIDENTIFIER NULL,
    [AvailableInServiceSelectionStep] BIT              NULL,
    [SelectedByDefault]               BIT              NULL,
    [Required]                        BIT              NULL,
    [CanAddMultipleServices]          BIT              NULL,
    [DecreeID]                        INT              NULL,
    [CreatedTime]                     DATETIME         NULL,
    [CreatedBy]                       INT              NULL,
    [LastModifiedTime]                DATETIME         NULL,
    [LastModifiedBy]                  INT              NULL,
    [timestamp]                       ROWVERSION       NULL,
    [HasOptions]                      BIT              NULL,
    [Rank]                            INT              NULL,
    [OptionsRestrictedToTechnology]   BIT              NULL,
    CONSTRAINT [PK__Contract__3214EC272610A626] PRIMARY KEY CLUSTERED ([ID] ASC)
);



