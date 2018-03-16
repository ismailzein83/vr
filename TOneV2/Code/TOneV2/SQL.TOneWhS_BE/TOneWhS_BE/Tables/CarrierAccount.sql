CREATE TABLE [TOneWhS_BE].[CarrierAccount] (
    [ID]                     INT            IDENTITY (1, 1) NOT NULL,
    [NameSuffix]             NVARCHAR (255) NULL,
    [CarrierProfileID]       INT            NOT NULL,
    [AccountType]            INT            NOT NULL,
    [SupplierSettings]       NVARCHAR (MAX) NULL,
    [CustomerSettings]       NVARCHAR (MAX) NULL,
    [CarrierAccountSettings] NVARCHAR (MAX) NULL,
    [ExtendedSettings]       NVARCHAR (MAX) NULL,
    [SellingNumberPlanID]    INT            NULL,
    [SellingProductID]       INT            NULL,
    [timestamp]              ROWVERSION     NULL,
    [SourceID]               VARCHAR (50)   NULL,
    [IsDeleted]              BIT            NULL,
    [CreatedTime]            DATETIME       CONSTRAINT [DF_CarrierAccount_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]              INT            NULL,
    [LastModifiedBy]         INT            NULL,
    [LastModifiedTime]       DATETIME       NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CarrierAccount_CarrierProfile] FOREIGN KEY ([CarrierProfileID]) REFERENCES [TOneWhS_BE].[CarrierProfile] ([ID]),
    CONSTRAINT [FK_CarrierAccount_SellingNumberPlan] FOREIGN KEY ([SellingNumberPlanID]) REFERENCES [TOneWhS_BE].[SellingNumberPlan] ([ID]),
    CONSTRAINT [FK_CarrierAccount_SellingProduct] FOREIGN KEY ([SellingProductID]) REFERENCES [TOneWhS_BE].[SellingProduct] ([ID])
);

















