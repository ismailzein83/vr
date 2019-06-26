CREATE TABLE [RA_Retail_SMSAnalytics].[ProcessedPostpaidSMSBillingStatsDaily] (
    [ID]             BIGINT           NOT NULL,
    [Scope]          INT              NULL,
    [DataSourceID]   UNIQUEIDENTIFIER NULL,
    [SenderMSISDN]   NVARCHAR (255)   NULL,
    [ReceiverMSISDN] NVARCHAR (255)   NULL,
    [OperatorID]     BIGINT           NULL,
    [SubscriberID]   BIGINT           NULL,
    [TotalRevenue]   DECIMAL (22, 8)  NULL,
    [TotalIncome]    DECIMAL (22, 8)  NULL,
    [NumberOfSMS]    INT              NULL,
    [BatchStart]     DATETIME         NULL,
    CONSTRAINT [PK_ProcessedPostpaidSMSBillingStatsDaily] PRIMARY KEY CLUSTERED ([ID] ASC)
);

