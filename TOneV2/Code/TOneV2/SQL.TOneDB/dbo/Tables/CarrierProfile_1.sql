﻿CREATE TABLE [dbo].[CarrierProfile] (
    [ProfileID]              SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]                   NVARCHAR (200) NULL,
    [CompanyName]            NVARCHAR (200) NULL,
    [CompanyLogo]            IMAGE          NULL,
    [CompanyLogoName]        NVARCHAR (500) NULL,
    [Address1]               NVARCHAR (250) NULL,
    [Country]                NVARCHAR (100) NULL,
    [Telephone]              NVARCHAR (100) NULL,
    [Fax]                    NVARCHAR (100) NULL,
    [BillingContact]         NVARCHAR (200) NULL,
    [BillingEmail]           NVARCHAR (256) NULL,
    [PricingContact]         NVARCHAR (200) NULL,
    [PricingEmail]           NVARCHAR (256) NULL,
    [SupportContact]         NVARCHAR (200) NULL,
    [SupportEmail]           NVARCHAR (256) NULL,
    [CurrencyID]             VARCHAR (3)    NULL,
    [DuePeriod]              TINYINT        NULL,
    [PaymentTerms]           TINYINT        NULL,
    [Tax1]                   NUMERIC (6, 2) NULL,
    [Tax2]                   NUMERIC (6, 2) NULL,
    [IsTaxAffectsCost]       CHAR (1)       NULL,
    [TaxFormula]             VARCHAR (200)  NULL,
    [VAT]                    NUMERIC (6, 2) NULL,
    [Services]               NUMERIC (6, 2) NULL,
    [ConnectionFees]         MONEY          NULL,
    [IsDeleted]              CHAR (1)       NULL,
    [BankingDetails]         NTEXT          NULL,
    [UserID]                 INT            NULL,
    [timestamp]              ROWVERSION     NULL,
    [RegistrationNumber]     NVARCHAR (50)  NULL,
    [EscalationLevel]        NVARCHAR (MAX) NULL,
    [Guarantee]              FLOAT (53)     NULL,
    [CustomerPaymentType]    TINYINT        NULL,
    [SupplierPaymentType]    TINYINT        NULL,
    [CustomerCreditLimit]    INT            NULL,
    [SupplierCreditLimit]    INT            NULL,
    [IsNettingEnabled]       CHAR (1)       CONSTRAINT [DF_CarrierProfile_IsNettingEnabled] DEFAULT ('N') NULL,
    [CustomerActivateDate]   DATETIME       NULL,
    [CustomerDeactivateDate] DATETIME       NULL,
    [SupplierActivateDate]   DATETIME       NULL,
    [SupplierDeactivateDate] DATETIME       NULL,
    [VatID]                  VARCHAR (20)   NULL,
    [AccountManagerEmail]    NVARCHAR (255) NULL,
    [InvoiceByProfile]       CHAR (1)       CONSTRAINT [DF_CarrierProfile_InvoiceByProfile] DEFAULT ('N') NULL,
    [Address2]               NVARCHAR (100) NULL,
    [Address3]               NVARCHAR (100) NULL,
    [City]                   NVARCHAR (100) NULL,
    [VatOffice]              VARCHAR (50)   NULL,
    [SMSPhoneNumber]         VARCHAR (50)   NULL,
    [CustomerSMSOnPayment]   CHAR (1)       NULL,
    [CustomerMailOnPayment]  CHAR (1)       NULL,
    [SupplierSMSOnPayment]   CHAR (1)       NULL,
    [SupplierMailOnPayment]  CHAR (1)       NULL,
    [Website]                VARCHAR (255)  NULL,
    [BillingDisputeEmail]    NVARCHAR (256) NULL,
    [CustomerAllowPayment]   CHAR (1)       NULL,
    [SupplierAllowPayment]   CHAR (1)       NULL,
    [TechnicalContact]       NVARCHAR (200) NULL,
    [TechnicalEmail]         NVARCHAR (256) NULL,
    [CommercialContact]      NVARCHAR (200) NULL,
    [CommercialEmail]        NVARCHAR (250) NULL,
    [AccountManagerContact]  NVARCHAR (200) NULL,
    CONSTRAINT [PK_CarrierProfile] PRIMARY KEY CLUSTERED ([ProfileID] ASC)
);

