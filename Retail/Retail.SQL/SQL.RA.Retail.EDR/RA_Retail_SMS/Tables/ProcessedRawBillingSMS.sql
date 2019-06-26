CREATE TABLE [RA_Retail_SMS].[ProcessedRawBillingSMS] (
    [ID]              BIGINT           NOT NULL,
    [AttemptDateTime] DATETIME         NULL,
    [Scope]           INT              NULL,
    [SenderMSISDN]    NVARCHAR (255)   NULL,
    [ReceiverMSISDN]  NVARCHAR (255)   NULL,
    [Revenue]         DECIMAL (22, 8)  NULL,
    [DataSourceID]    UNIQUEIDENTIFIER NULL,
    [OperatorID]      BIGINT           NULL,
    [SubscriberType]  INT              NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

