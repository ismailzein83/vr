CREATE TABLE [TOneWhS_BE].[CarrierAccount] (
    [ID]                     INT            IDENTITY (1, 1) NOT NULL,
    [Name]                   NVARCHAR (255) NOT NULL,
    [CarrierProfileID]       INT            NOT NULL,
    [AccountType]            INT            NOT NULL,
    [SupplierSettings]       NVARCHAR (MAX) NULL,
    [CustomerSettings]       NVARCHAR (MAX) NULL,
    [CarrierAccountSettings] NVARCHAR (MAX) NULL,
    [SellingNumberPlanID]    INT            NULL,
    [timestamp]              ROWVERSION     NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CarrierAccount_CarrierProfile] FOREIGN KEY ([CarrierProfileID]) REFERENCES [TOneWhS_BE].[CarrierProfile] ([ID])
);



