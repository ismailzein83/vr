CREATE TYPE [RA_Analytics].[TrafficStats15MinType] AS TABLE (
    [ID]                 BIGINT           NULL,
    [BatchStart]         DATETIME         NULL,
    [OperatorID]         BIGINT           NULL,
    [NumberOfCDRs]       INT              NULL,
    [TrafficDirection]   INT              NULL,
    [SuccessfulAttempts] INT              NULL,
    [SumOfPDDInSeconds]  DECIMAL (20, 4)  NULL,
    [DurationInSeconds]  DECIMAL (20, 4)  NULL,
    [ZoneID]             BIGINT           NULL,
    [Country]            INT              NULL,
    [DataSource]         UNIQUEIDENTIFIER NULL,
    [Probe]              BIGINT           NULL,
    [Attempts]           INT              NULL,
    [DeliveredAttempts]  INT              NULL);



