CREATE TABLE [RA_Retail_CDR].[ProcessedRawBillingCDR] (
    [ID]              BIGINT           NOT NULL,
    [AttemptDateTime] DATETIME         NULL,
    [Scope]           INT              NULL,
    [Revenue]         DECIMAL (22, 8)  NULL,
    [CallingMSISDN]   NVARCHAR (255)   NULL,
    [CalledMSISDN]    NVARCHAR (255)   NULL,
    [DataSourceID]    UNIQUEIDENTIFIER NULL,
    [OperatorID]      BIGINT           NULL,
    [Duration]        DECIMAL (20, 4)  NULL,
    [SubscriberType]  INT              NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

