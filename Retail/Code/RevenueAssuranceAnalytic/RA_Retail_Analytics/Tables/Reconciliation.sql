CREATE TABLE [RA_Retail_Analytics].[Reconciliation] (
    [ID]                       BIGINT          IDENTITY (1, 1) NOT NULL,
    [BatchStart]               DATETIME        NULL,
    [OperatorID]               BIGINT          NULL,
    [AccountType]              INT             NULL,
    [ServiceType]              INT             NULL,
    [TrafficType]              INT             NULL,
    [DeclaredRevenue]          DECIMAL (30, 4) NULL,
    [MeasuredRevenue]          DECIMAL (30, 4) NULL,
    [DeclaredDuration]         DECIMAL (30, 4) NULL,
    [MeasuredDuration]         DECIMAL (30, 4) NULL,
    [DeclaredSuccessfulEvents] DECIMAL (30, 4) NULL,
    [MeasuredSuccessfulEvents] DECIMAL (30, 4) NULL,
    [RevenueTolerance]         DECIMAL (30, 4) NULL,
    CONSTRAINT [IX_RA_Retail_Reconciliation_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);

