CREATE TABLE [FraudAnalysis].[NumberProfile] (
    [ID]              INT            IDENTITY (1, 1) NOT NULL,
    [AccountNumber]   VARCHAR (30)   NULL,
    [FromDate]        DATETIME       NULL,
    [ToDate]          DATETIME       NULL,
    [AggregateValues] NVARCHAR (MAX) NULL,
    [IMEIs]           NVARCHAR (MAX) NULL
);










GO



GO
CREATE NONCLUSTERED INDEX [IX_NumberProfile_AccountNumber]
    ON [FraudAnalysis].[NumberProfile]([AccountNumber] ASC);

