CREATE TABLE [RA_Retail].[Subscriber] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [MSISDN]           NVARCHAR (255)   NULL,
    [IMSI]             NVARCHAR (255)   NULL,
    [IMEI]             NVARCHAR (255)   NULL,
    [TMSI]             NVARCHAR (255)   NULL,
    [MSRN]             NVARCHAR (255)   NULL,
    [StatusId]         UNIQUEIDENTIFIER NULL,
    [SIMToTerminal]    DATETIME         NULL,
    [SubscriberType]   INT              NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_Subscriber_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [RegistrationDate] DATETIME         NULL,
    [OperatorId]       BIGINT           NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__Subscrib__3214EC27619B8048] PRIMARY KEY CLUSTERED ([ID] ASC)
);

