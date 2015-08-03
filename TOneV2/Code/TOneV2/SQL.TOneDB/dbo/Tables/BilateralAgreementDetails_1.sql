CREATE TABLE [dbo].[BilateralAgreementDetails] (
    [ID]                    INT            IDENTITY (1, 1) NOT NULL,
    [GroupName]             NVARCHAR (50)  NULL,
    [SupplierID]            VARCHAR (5)    NULL,
    [AgreementID]           INT            NOT NULL,
    [ASR]                   FLOAT (53)     NULL,
    [ACD]                   FLOAT (53)     NULL,
    [Rate]                  FLOAT (53)     NOT NULL,
    [Volume]                INT            NOT NULL,
    [Amount]                FLOAT (53)     NULL,
    [NER]                   FLOAT (53)     NULL,
    [MaxCostRate]           FLOAT (53)     NULL,
    [MinSaleRate]           FLOAT (53)     NULL,
    [SubstituteRate]        FLOAT (53)     NULL,
    [Config]                NVARCHAR (MAX) NULL,
    [IsRoutingRulesApplied] CHAR (1)       CONSTRAINT [DF_BilateralAgreementDetails_IsRoutingRulesApplied] DEFAULT ('N') NULL,
    [OverrideActionApplied] CHAR (1)       CONSTRAINT [DF_BilateralAgreementDetails_OverrideActionApplied] DEFAULT ('N') NULL,
    CONSTRAINT [PK_BilateralAgreementZones] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_AgreementID]
    ON [dbo].[BilateralAgreementDetails]([AgreementID] ASC);

