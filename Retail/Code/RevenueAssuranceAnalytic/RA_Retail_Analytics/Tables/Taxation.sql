CREATE TABLE [RA_Retail_Analytics].[Taxation] (
    [ID]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [BatchStart]    DATETIME        NULL,
    [OperatorID]    BIGINT          NULL,
    [AccountType]   INT             NULL,
    [BillingSource] INT             NULL,
    [NumberOfCalls] INT             NULL,
    [Duration]      DECIMAL (30, 4) NULL,
    [SMS]           DECIMAL (30, 4) NULL,
    [Data]          DECIMAL (30, 4) NULL,
    [Amount]        DECIMAL (30, 4) NULL,
    [Income]        DECIMAL (30, 4) NULL,
    CONSTRAINT [IX_RA_Retail_Taxation_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);

