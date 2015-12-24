CREATE TABLE [QM_CLITester].[TestCall] (
    [ID]                      BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserID]                  INT            NULL,
    [ProfileID]               INT            NULL,
    [SupplierID]              INT            NULL,
    [CountryID]               INT            NULL,
    [ZoneID]                  INT            NULL,
    [CreationDate]            DATETIME       NULL,
    [CallTestStatus]          INT            NULL,
    [CallTestResult]          INT            NULL,
    [InitiateTestInformation] NVARCHAR (MAX) NULL,
    [TestProgress]            NVARCHAR (MAX) NULL,
    [InitiationRetryCount]    INT            NULL,
    [GetProgressRetryCount]   INT            NULL,
    [FailureMessage]          NVARCHAR (MAX) NULL,
    [timestamp]               ROWVERSION     NULL,
    [BatchNumber]             BIGINT         NULL,
    CONSTRAINT [PK_TestCall] PRIMARY KEY CLUSTERED ([ID] ASC)
);





