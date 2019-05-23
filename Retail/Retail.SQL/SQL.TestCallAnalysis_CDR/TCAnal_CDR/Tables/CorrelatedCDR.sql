CREATE TABLE [TCAnal_CDR].[CorrelatedCDR] (
    [ID]                         BIGINT          NOT NULL,
    [AttemptDateTime]            DATETIME        NULL,
    [ClientId]                   BIGINT          NULL,
    [GeneratedCallingNumber]     VARCHAR (40)    NULL,
    [ReceivedCallingNumber]      VARCHAR (40)    NULL,
    [CaseId]                     BIGINT          NULL,
    [OriginatedGeneratedZoneId]  BIGINT          NULL,
    [CallingOperatorID]          BIGINT          NULL,
    [OriginatedReceivedZoneId]   BIGINT          NULL,
    [CalledOperatorID]           BIGINT          NULL,
    [DurationInSeconds]          DECIMAL (20, 4) NULL,
    [ReceivedCallingNumberType]  INT             NULL,
    [CreatedTime]                DATETIME        NULL,
    [GeneratedId]                BIGINT          NULL,
    [ReceivedId]                 BIGINT          NULL,
    [GeneratedCalledNumber]      NVARCHAR (255)  NULL,
    [ReceivedCalledNumber]       NVARCHAR (255)  NULL,
    [OrigGeneratedCallingNumber] NVARCHAR (255)  NULL,
    [OrigGeneratedCalledNumber]  NVARCHAR (255)  NULL,
    [OrigReceivedCalledNumber]   NVARCHAR (255)  NULL,
    [OrigReceivedCallingNumber]  NVARCHAR (255)  NULL,
    [timestamp]                  ROWVERSION      NULL
);







