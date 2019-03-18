CREATE TABLE [RA_Retail_Analytics].[TrafficStats15Min] (
    [ID]                     BIGINT          IDENTITY (1, 1) NOT NULL,
    [BatchStart]             DATETIME        NULL,
    [OperatorID]             BIGINT          NULL,
    [NumberOfCalls]          INT             NULL,
    [SuccessfulAttempts]     BIGINT          NULL,
    [TotalDurationInSeconds] DECIMAL (30, 4) NULL,
    [PDDInSeconds]           DECIMAL (20, 4) NULL,
    [DCR]                    DECIMAL (20, 4) NULL,
    CONSTRAINT [IX_RA_ICX_TrafficStats15Min_ID] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);

