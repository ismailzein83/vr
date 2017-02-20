CREATE TABLE [TOneWhS_AccBalance].[CarrierFinancialAccount] (
    [ID]                       INT            IDENTITY (1, 1) NOT NULL,
    [CarrierProfileId]         INT            NULL,
    [CarrierAccountId]         INT            NULL,
    [FinancialAccountSettings] NVARCHAR (MAX) NULL,
    [BED]                      DATETIME       NOT NULL,
    [EED]                      DATETIME       NULL,
    [CreatedTime]              DATETIME       CONSTRAINT [DF_CarrierFinancialAccount_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                ROWVERSION     NULL,
    CONSTRAINT [PK_CarrierFinancialAccount] PRIMARY KEY CLUSTERED ([ID] ASC)
);

