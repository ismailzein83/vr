CREATE TABLE [dbo].[CarrierAccount] (
    [CarrierAccountID]           VARCHAR (5)    NOT NULL,
    [ProfileID]                  SMALLINT       NOT NULL,
    [ServicesFlag]               SMALLINT       NOT NULL,
    [ActivationStatus]           TINYINT        NOT NULL,
    [RoutingStatus]              TINYINT        NOT NULL,
    [AccountType]                TINYINT        NOT NULL,
    [CustomerPaymentType]        TINYINT        NOT NULL,
    [SupplierCreditLimit]        INT            NULL,
    [BillingCycleFrom]           SMALLINT       NULL,
    [BillingCycleTo]             SMALLINT       NULL,
    [GMTTime]                    SMALLINT       NULL,
    [IsTOD]                      CHAR (1)       NULL,
    [IsDeleted]                  CHAR (1)       NULL,
    [IsOriginatingZonesEnabled]  CHAR (1)       CONSTRAINT [DF_CarrierAccount_IsOriginatingZonesEnabled] DEFAULT ('N') NULL,
    [Notes]                      TEXT           NULL,
    [NominalCapacityInE1s]       INT            NULL,
    [UserID]                     INT            NULL,
    [timestamp]                  ROWVERSION     NULL,
    [CarrierGroupID]             INT            NULL,
    [RateIncreaseDays]           INT            NULL,
    [BankReferences]             VARCHAR (MAX)  NULL,
    [SupplierPaymentType]        TINYINT        NULL,
    [CustomerCreditLimit]        INT            NULL,
    [IsPassThroughCustomer]      CHAR (1)       CONSTRAINT [DF_CarrierAccount_IsPassThroughCustomer] DEFAULT ('N') NULL,
    [IsPassThroughSupplier]      CHAR (1)       CONSTRAINT [DF_CarrierAccount_IsPassThroughSupplier] DEFAULT ('N') NULL,
    [RepresentsASwitch]          CHAR (1)       CONSTRAINT [DF_CarrierAccount_RepresentsASwitch] DEFAULT ('N') NULL,
    [IsAToZ]                     CHAR (1)       CONSTRAINT [DF_CarrierAccount_IsAToZ] DEFAULT ('N') NULL,
    [NameSuffix]                 NVARCHAR (100) NULL,
    [SupplierRatePolicy]         INT            NULL,
    [CustomerGMTTime]            SMALLINT       NULL,
    [ImportEmail]                VARCHAR (320)  NULL,
    [ImportSubjectCode]          VARCHAR (50)   NULL,
    [IsNettingEnabled]           CHAR (1)       CONSTRAINT [DF_CarrierAccount_IsNettingEnabled] DEFAULT ('N') NULL,
    [Services]                   NUMERIC (6, 2) NULL,
    [ConnectionFees]             MONEY          NULL,
    [CustomerActivateDate]       DATETIME       NULL,
    [CustomerDeactivateDate]     DATETIME       NULL,
    [SupplierActivateDate]       DATETIME       NULL,
    [SupplierDeactivateDate]     DATETIME       NULL,
    [InvoiceSerialPattern]       NVARCHAR (300) NULL,
    [CustomerMask]               VARCHAR (10)   NULL,
    [IsCustomerCeiling]          CHAR (1)       CONSTRAINT [DF_CarrierAccount_IsCustomerCeiling] DEFAULT ('N') NULL,
    [IsSupplierCeiling]          CHAR (1)       CONSTRAINT [DF_CarrierAccount_IsSupplierCeiling] DEFAULT ('N') NULL,
    [CarrierGroups]              VARCHAR (50)   NULL,
    [CodeView]                   INT            NULL,
    [IsCustomCodeView]           CHAR (1)       NULL,
    [AutomaticInvoiceSettingID]  INT            NULL,
    [CarrierMask]                VARCHAR (50)   NULL,
    [IsProduct]                  CHAR (1)       CONSTRAINT [DF_CarrierAccount_IsProduct] DEFAULT ('N') NULL,
    [CustomerSMSOnPayment]       CHAR (1)       NULL,
    [CustomerMailOnPayment]      CHAR (1)       NULL,
    [SupplierSMSOnPayment]       CHAR (1)       NULL,
    [SupplierMailOnPayment]      CHAR (1)       NULL,
    [IsCustomDispute]            CHAR (1)       NULL,
    [CustomerAllowPayment]       CHAR (1)       NULL,
    [SupplierAllowPayment]       CHAR (1)       NULL,
    [AutomatedImportSubjectCode] VARCHAR (50)   NULL,
    [IsLCREnabled]               CHAR (1)       CONSTRAINT [DF_CarrierAccount_IsLCREnabled] DEFAULT ('N') NULL,
    [DS_ID_auto]                 INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_CarrierAccount] PRIMARY KEY CLUSTERED ([CarrierAccountID] ASC),
    CONSTRAINT [FK_CarrierAccount_CarrierGroup] FOREIGN KEY ([CarrierGroupID]) REFERENCES [dbo].[CarrierGroup] ([CarrierGroupID]) ON DELETE SET NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_CarrierAccount]
    ON [dbo].[CarrierAccount]([ProfileID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CarrierAccount_1]
    ON [dbo].[CarrierAccount]([IsDeleted] ASC);

