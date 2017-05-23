CREATE TABLE [Retail_EDR].[RingoEvent] (
    [MSISDN]                 NVARCHAR (100)   NULL,
    [EventIdMvno]            INT              NULL,
    [EventId]                INT              NULL,
    [Event]                  NVARCHAR (100)   NULL,
    [Parameters]             NVARCHAR (100)   NULL,
    [CreatedDate]            DATETIME         NULL,
    [AccountId]              BIGINT           NULL,
    [FileName]               NVARCHAR (200)   NULL,
    [PromotionCode]          NVARCHAR (200)   NULL,
    [PromotionId]            INT              NULL,
    [ActivationDate]         DATETIME         NULL,
    [CustomerActivationDate] DATETIME         NULL,
    [PackagePrice]           DECIMAL (20, 10) NULL,
    [Amount]                 INT              NULL,
    [Balance]                DECIMAL (20, 10) NULL
);









