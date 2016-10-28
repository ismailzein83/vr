CREATE TABLE [QM_CLITester].[TestCall] (
    [ID]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserID]                  INT              NULL,
    [ProfileID]               INT              NULL,
    [SupplierID]              INT              NULL,
    [CountryID]               INT              NULL,
    [ZoneID]                  BIGINT           NULL,
    [CreationDate]            DATETIME         NULL,
    [CallTestStatus]          INT              NULL,
    [CallTestResult]          INT              NULL,
    [InitiateTestInformation] NVARCHAR (MAX)   NULL,
    [TestProgress]            NVARCHAR (MAX)   NULL,
    [InitiationRetryCount]    INT              NULL,
    [GetProgressRetryCount]   INT              NULL,
    [FailureMessage]          NVARCHAR (MAX)   NULL,
    [timestamp]               ROWVERSION       NULL,
    [BatchNumber]             BIGINT           NULL,
    [OldScheduleID]           INT              NULL,
    [PDD]                     DECIMAL (18, 6)  NULL,
    [MOS]                     DECIMAL (18, 6)  NULL,
    [Duration]                DATETIME         NULL,
    [RingDuration]            VARCHAR (20)     NULL,
    [UpdateStatusTime]        DATETIME         NULL,
    [Quantity]                INT              NULL,
    [ScheduleID]              UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_TestCall] PRIMARY KEY CLUSTERED ([ID] ASC)
);



















