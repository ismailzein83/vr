CREATE TABLE [FraudAnalysis].[StagingCDR] (
    [CGPN]               VARCHAR (40)    NOT NULL,
    [CDPN]               VARCHAR (40)    NOT NULL,
    [SwitchID]           INT             NULL,
    [InTrunkSymbol]      NVARCHAR (50)   NULL,
    [OutTrunkSymbol]     NVARCHAR (50)   NULL,
    [ConnectDateTime]    DATETIME        NOT NULL,
    [DurationInSeconds]  NUMERIC (13, 4) NOT NULL,
    [DisconnectDateTime] DATETIME        NULL,
    [InTrunkID]          INT             NULL,
    [OutTrunkID]         INT             NULL,
    [CGPNAreaCode]       VARCHAR (10)    NULL,
    [CDPNAreaCode]       VARCHAR (10)    NULL
);






GO
CREATE NONCLUSTERED INDEX [IX_StagingCDRS_ConnectDateTime]
    ON [FraudAnalysis].[StagingCDR]([ConnectDateTime] ASC);

