CREATE TABLE [RA_Retail_CDR].[ProcessedBillingPrepaidCDR] (
    [ID]              BIGINT           IDENTITY (1, 1) NOT NULL,
    [AttemptDateTime] DATETIME         NULL,
    [Scope]           INT              NULL,
    [CallingMSISDN]   NVARCHAR (255)   NULL,
    [CalledMSISDN]    NVARCHAR (255)   NULL,
    [Duration]        DECIMAL (20, 4)  NULL,
    [Revenue]         DECIMAL (22, 8)  NULL,
    [DataSourceID]    UNIQUEIDENTIFIER NULL,
    [OperatorID]      BIGINT           NULL,
    [SubscriberID]    BIGINT           NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

