CREATE TABLE [RetailBilling].[RatePlanCharge_Voice] (
    [ID]              INT             NOT NULL,
    [RecurringCharge] DECIMAL (20, 8) NULL,
    [Deposit]         DECIMAL (20, 8) NULL,
    [timestamp]       ROWVERSION      NULL,
    CONSTRAINT [PK_RatePlanCharge_Voice] PRIMARY KEY CLUSTERED ([ID] ASC)
);

