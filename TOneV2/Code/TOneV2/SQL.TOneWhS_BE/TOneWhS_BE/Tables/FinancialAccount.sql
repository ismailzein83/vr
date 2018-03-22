CREATE TABLE [TOneWhS_BE].[FinancialAccount] (
    [ID]                           INT              IDENTITY (1, 1) NOT NULL,
    [CarrierProfileId]             INT              NULL,
    [CarrierAccountId]             INT              NULL,
    [FinancialAccountDefinitionId] UNIQUEIDENTIFIER NOT NULL,
    [FinancialAccountSettings]     NVARCHAR (MAX)   NULL,
    [BED]                          DATETIME         NOT NULL,
    [EED]                          DATETIME         NULL,
    [CreatedTime]                  DATETIME         CONSTRAINT [DF_FinancialAccount_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                    ROWVERSION       NULL,
    [CreatedBy]                    INT              NULL,
    [LastModifiedBy]               INT              NULL,
    [LastModifiedTime]             DATETIME         NULL,
    CONSTRAINT [PK_FinancialAccount] PRIMARY KEY CLUSTERED ([ID] ASC)
);





