CREATE TYPE [Retail_EDR].[RingoEventType] AS TABLE (
    [MSISDN]      NVARCHAR (100) NULL,
    [EventIdMvno] INT            NULL,
    [EventId]     INT            NULL,
    [Event]       NVARCHAR (100) NULL,
    [Parameters]  NVARCHAR (100) NULL,
    [CreatedDate] DATETIME       NULL,
    [AccountId]   BIGINT         NULL);



