CREATE TABLE [dbo].[PSTNSampleData] (
    [ID]                 INT             IDENTITY (1, 1) NOT NULL,
    [CGPN]               VARCHAR (40)    NOT NULL,
    [CDPN]               VARCHAR (40)    NOT NULL,
    [InTrunkSymbol]      NVARCHAR (50)   NULL,
    [OutTrunkSymbol]     NVARCHAR (50)   NULL,
    [ConnectDateTime]    DATETIME        NOT NULL,
    [DurationInSeconds]  NUMERIC (13, 4) NOT NULL,
    [DisconnectDateTime] DATETIME        NULL,
    CONSTRAINT [PK_PSTNSampleData] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_StagingCDR_CDPN_CGPN]
    ON [dbo].[PSTNSampleData]([CGPN] ASC, [CDPN] ASC);

