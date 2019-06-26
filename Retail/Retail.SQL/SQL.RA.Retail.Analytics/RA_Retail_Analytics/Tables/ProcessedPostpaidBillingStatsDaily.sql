CREATE TABLE [RA_Retail_Analytics].[ProcessedPostpaidBillingStatsDaily] (
    [ID]            BIGINT           NOT NULL,
    [Scope]         INT              NULL,
    [DataSourceID]  UNIQUEIDENTIFIER NULL,
    [CallingMSISDN] NVARCHAR (255)   NULL,
    [CalledMSISDN]  NVARCHAR (255)   NULL,
    [OperatorID]    BIGINT           NULL,
    [SubscriberID]  BIGINT           NULL,
    [TotalDuration] DECIMAL (20, 4)  NULL,
    [TotalRevenue]  DECIMAL (22, 8)  NULL,
    [TotalIncome]   DECIMAL (22, 8)  NULL,
    [NumberOfCalls] INT              NULL,
    [BatchStart]    DATETIME         NULL,
    CONSTRAINT [PK_ProcessedPostpaidBillingStatsDaily] PRIMARY KEY CLUSTERED ([ID] ASC)
);

