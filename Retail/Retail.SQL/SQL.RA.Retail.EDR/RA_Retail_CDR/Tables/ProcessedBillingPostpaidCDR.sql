CREATE TABLE [RA_Retail_CDR].[ProcessedBillingPostpaidCDR] (
    [ID]              BIGINT           NOT NULL,
    [AttemptDateTime] DATETIME         NULL,
    [Scope]           INT              NULL,
    [DataSourceID]    UNIQUEIDENTIFIER NULL,
    [CallingMSISDN]   NVARCHAR (255)   NULL,
    [CalledMSISDN]    NVARCHAR (255)   NULL,
    [Duration]        DECIMAL (20, 4)  NULL,
    [Revenue]         DECIMAL (22, 8)  NULL,
    [Income]          DECIMAL (22, 8)  NULL,
    [OperatorID]      BIGINT           NULL,
    [SubscriberID]    BIGINT           NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

