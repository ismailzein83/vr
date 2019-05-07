CREATE TABLE [TCAnal_CDR].[CorrelatedCDR] (
    [ID]                              BIGINT          NULL,
    [AttemptDateTime]                 DATETIME        NULL,
    [GeneratedCallingNumber]          VARCHAR (40)    NULL,
    [ReceivedCallingNumber]           VARCHAR (40)    NULL,
    [CaseId]                          BIGINT          NULL,
    [OperatorID]                      BIGINT          NULL,
    [DurationInSeconds]               DECIMAL (20, 4) NULL,
    [ReceivedCallingNumberType]       INT             NULL,
    [ReceivedCallingNumberOperatorID] BIGINT          NULL,
    [CreatedTime]                     DATETIME        NULL,
    [timestamp]                       ROWVERSION      NULL,
    [GeneratedCalledNumber]           NVARCHAR (255)  NULL,
    [ReceivedCalledNumber]            NVARCHAR (255)  NULL,
    [OrigGeneratedCallingNumber]      NVARCHAR (255)  NULL,
    [OrigGeneratedCalledNumber]       NVARCHAR (255)  NULL,
    [OrigReceivedCalledNumber]        NVARCHAR (255)  NULL,
    [OrigReceivedCallingNumber]       NVARCHAR (255)  NULL
);





