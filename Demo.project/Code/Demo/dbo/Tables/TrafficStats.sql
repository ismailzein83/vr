CREATE TABLE [dbo].[TrafficStats] (
    [ID]                     BIGINT          NULL,
    [OperatorID]             INT             NULL,
    [Attempts]               INT             NULL,
    [DurationInSeconds]      NUMERIC (20, 6) NULL,
    [FirstCDRAttempt]        DATETIME        NULL,
    [LastCDRAttempt]         DATETIME        NULL,
    [ZoneID]                 BIGINT          NULL,
    [SumOfPDDInSeconds]      NUMERIC (20, 6) NULL,
    [MaxDurationInSeconds]   NUMERIC (20, 6) NULL,
    [NumberOfCalls]          INT             NULL,
    [PortOut]                NVARCHAR (50)   NULL,
    [PortIn]                 NVARCHAR (50)   NULL,
    [DeliveredAttempts]      INT             NULL,
    [SuccessfulAttempts]     INT             NULL,
    [DeliveredNumberOfCalls] INT             NOT NULL,
    [CeiledDuration]         BIGINT          NOT NULL,
    [SumOfPGAD]              NUMERIC (20, 6) NULL,
    [UtilizationInSeconds]   NUMERIC (20, 6) NULL,
    [ServiceTypeID]          INT             NULL,
    [Direction]              INT             NULL,
    [CDRType]                INT             NOT NULL,
    CONSTRAINT [IX_TrafficStats_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_TrafficStats_DateTimeFirst]
    ON [dbo].[TrafficStats]([FirstCDRAttempt] ASC);

