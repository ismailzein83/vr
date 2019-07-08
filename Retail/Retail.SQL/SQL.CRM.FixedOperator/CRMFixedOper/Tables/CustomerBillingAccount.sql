﻿CREATE TABLE [CRMFixedOper].[CustomerBillingAccount] (
    [ID]                 BIGINT           NOT NULL,
    [CustomerID]         BIGINT           NULL,
    [Name]               NVARCHAR (255)   NULL,
    [RegionID]           INT              NULL,
    [CityID]             INT              NULL,
    [TownID]             INT              NULL,
    [Street]             NVARCHAR (255)   NULL,
    [Building]           NVARCHAR (255)   NULL,
    [FloorNumber]        INT              NULL,
    [AddressNotes]       NVARCHAR (MAX)   NULL,
    [CreatedTime]        DATETIME         NULL,
    [CreatedBy]          INT              NULL,
    [LastModifiedTime]   DATETIME         NULL,
    [LastModifiedBy]     INT              NULL,
    [CurrencyID]         INT              NULL,
    [InvoiceSettingId]   UNIQUEIDENTIFIER NULL,
    [PaymentType]        INT              NULL,
    [AccountNumber]      NVARCHAR (255)   NULL,
    [BankId]             INT              NULL,
    [PaymentAccountType] INT              NULL,
    [BranchId]           INT              NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);



