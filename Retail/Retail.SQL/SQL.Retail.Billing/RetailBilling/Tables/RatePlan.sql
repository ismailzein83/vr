CREATE TABLE [RetailBilling].[RatePlan] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [ContractTypeID]   UNIQUEIDENTIFIER NULL,
    [CurrencyID]       INT              NULL,
    [AvailableFrom]    DATETIME         NULL,
    [AvailableTo]      DATETIME         NULL,
    [ActivationFee]    DECIMAL (20, 8)  NULL,
    [RecurringFee]     DECIMAL (20, 8)  NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__RatePlan__3214EC2707020F21] PRIMARY KEY CLUSTERED ([ID] ASC)
);

