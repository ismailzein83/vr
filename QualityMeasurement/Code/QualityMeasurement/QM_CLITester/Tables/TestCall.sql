CREATE TABLE [QM_CLITester].[TestCall] (
    [ID]             BIGINT         IDENTITY (1, 1) NOT NULL,
    [SupplierID]     INT            NULL,
    [CountryID]      INT            NULL,
    [ZoneID]         INT            NULL,
    [CreationDate]   DATETIME       NULL,
    [Test_ID]        NVARCHAR (50)  NULL,
    [Name]           NVARCHAR (100) NULL,
    [Calls_Total]    INT            NULL,
    [Calls_Complete] INT            NULL,
    [CLI_Success]    INT            NULL,
    [CLI_No_Result]  INT            NULL,
    [CLI_Fail]       INT            NULL,
    [PDD]            INT            NULL,
    [Share_URL]      NVARCHAR (500) NULL,
    [Status]         INT            NULL,
    CONSTRAINT [PK_TestCall] PRIMARY KEY CLUSTERED ([ID] ASC)
);

