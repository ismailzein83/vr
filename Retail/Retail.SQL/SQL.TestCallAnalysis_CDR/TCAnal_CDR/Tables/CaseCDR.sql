CREATE TABLE [TCAnal_CDR].[CaseCDR] (
    [ID]               BIGINT           NULL,
    [CallingNumber]    NVARCHAR (40)    NULL,
    [CalledNumber]     NVARCHAR (40)    NULL,
    [FirstAttempt]     DATETIME         NULL,
    [LastAttempt]      DATETIME         NULL,
    [NumberOfCDRs]     INT              NULL,
    [StatusId]         UNIQUEIDENTIFIER NULL,
    [OperatorID]       BIGINT           NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedTime] DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL
);

