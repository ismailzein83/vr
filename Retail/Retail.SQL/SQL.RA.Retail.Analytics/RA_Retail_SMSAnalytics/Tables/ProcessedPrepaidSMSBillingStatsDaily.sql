CREATE TABLE [RA_Retail_SMSAnalytics].[ProcessedPrepaidSMSBillingStatsDaily] (
    [ID]             BIGINT           NOT NULL,
    [Scope]          INT              NULL,
    [SenderMSISDN]   NVARCHAR (255)   NULL,
    [ReceiverMSISDN] NVARCHAR (255)   NULL,
    [DataSourceID]   UNIQUEIDENTIFIER NULL,
    [OperatorID]     BIGINT           NULL,
    [SubscriberID]   BIGINT           NULL,
    [TotalRevenue]   DECIMAL (22, 8)  NULL,
    [NumberOfSMS]    INT              NULL,
    [BatchStart]     DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

