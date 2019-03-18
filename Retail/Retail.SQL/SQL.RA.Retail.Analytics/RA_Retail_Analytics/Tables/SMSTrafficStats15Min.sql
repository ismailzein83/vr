CREATE TABLE [RA_Retail_Analytics].[SMSTrafficStats15Min] (
    [ID]                     BIGINT          IDENTITY (1, 1) NOT NULL,
    [BatchStart]             DATETIME        NULL,
    [OperatorID]             BIGINT          NULL,
    [NumberOfSMSs]           INT             NULL,
    [DeliveredSMSs]          INT             NULL,
    [AverageDelayInDelivery] DECIMAL (20, 4) NULL,
    CONSTRAINT [IX_RA_Retail_SMSTrafficStats15Min_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);

