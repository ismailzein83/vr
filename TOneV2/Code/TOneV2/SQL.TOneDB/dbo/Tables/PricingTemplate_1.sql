CREATE TABLE [dbo].[PricingTemplate] (
    [PricingTemplateId] INT           IDENTITY (1, 1) NOT NULL,
    [CurrencyID]        VARCHAR (3)   NOT NULL,
    [Title]             VARCHAR (100) NOT NULL,
    [Description]       VARCHAR (250) NULL,
    [UserID]            INT           NULL,
    [timestamp]         ROWVERSION    NULL,
    [TemplateType]      SMALLINT      NULL,
    CONSTRAINT [PK_PricingTemplate] PRIMARY KEY CLUSTERED ([PricingTemplateId] ASC)
);

