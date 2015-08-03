CREATE TABLE [dbo].[PricingTemplatePlan] (
    [PricingTemplatePlanId] INT             IDENTITY (1, 1) NOT NULL,
    [PricingTemplateID]     INT             NOT NULL,
    [ZoneID]                INT             NULL,
    [ServicesFlag]          SMALLINT        NULL,
    [FromPrice]             DECIMAL (13, 6) NOT NULL,
    [ToPrice]               DECIMAL (13, 6) NOT NULL,
    [Margin]                FLOAT (53)      NULL,
    [IsPerc]                CHAR (1)        NULL,
    [NotContinues]          CHAR (1)        NULL,
    [Priority]              SMALLINT        NULL,
    [Description]           VARCHAR (200)   NULL,
    [UserID]                INT             NULL,
    [timestamp]             ROWVERSION      NULL,
    [PricingReferenceRate]  INT             NULL,
    [SupplierID]            VARCHAR (5)     NULL,
    [Fixed]                 FLOAT (53)      NULL,
    [MaxMargin]             FLOAT (53)      NULL,
    [MinRate]               DECIMAL (13, 6) NULL,
    [MaxRate]               DECIMAL (13, 6) NULL,
    CONSTRAINT [PK_PricingTemplatePlan1] PRIMARY KEY CLUSTERED ([PricingTemplatePlanId] ASC),
    CONSTRAINT [FK_PricingTemplatePlan_PricingTemplate] FOREIGN KEY ([PricingTemplateID]) REFERENCES [dbo].[PricingTemplate] ([PricingTemplateId]) ON DELETE CASCADE
);

