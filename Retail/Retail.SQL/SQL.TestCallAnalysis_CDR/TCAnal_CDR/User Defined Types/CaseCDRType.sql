CREATE TYPE [TCAnal_CDR].[CaseCDRType] AS TABLE (
    [ID]            BIGINT           NULL,
    [CallingNumber] VARCHAR (40)     NULL,
    [CalledNumber]  VARCHAR (40)     NULL,
    [FirstAttempt]  DATETIME         NULL,
    [LastAttempt]   DATETIME         NULL,
    [NumberOdCDRs]  INT              NULL,
    [StatusId]      UNIQUEIDENTIFIER NULL,
    [OperatorID]    BIGINT           NULL);

