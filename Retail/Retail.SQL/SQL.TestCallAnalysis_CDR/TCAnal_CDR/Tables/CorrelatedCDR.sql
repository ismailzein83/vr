CREATE TABLE [TCAnal_CDR].[CorrelatedCDR] (
    [ID]                              BIGINT          NULL,
    [AttemptDateTime]                 DATETIME        NULL,
    [GeneratedCallingNumber]          VARCHAR (40)    NULL,
    [ReceivedCallingNumber]           VARCHAR (40)    NULL,
    [CalledNumber]                    VARCHAR (40)    NULL,
    [OrigCallingNumber]               VARCHAR (40)    NULL,
    [OrigCalledNumber]                VARCHAR (40)    NULL,
    [CaseId]                          BIGINT          NULL,
    [OperatorID]                      BIGINT          NULL,
    [DurationInSeconds]               DECIMAL (20, 4) NULL,
    [ReceivedCallingNumberType]       INT             NULL,
    [ReceivedCallingNumberOperatorID] BIGINT          NULL,
    [CreatedTime]                     DATETIME        NULL,
    [timestamp]                       ROWVERSION      NULL
);



