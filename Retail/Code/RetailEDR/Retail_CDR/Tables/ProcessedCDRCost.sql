CREATE TABLE [Retail_CDR].[ProcessedCDRCost] (
    [ID]                BIGINT          NULL,
    [AttemptDateTime]   DATETIME        NULL,
    [CGPN]              VARCHAR (100)   NULL,
    [CDPN]              VARCHAR (100)   NULL,
    [DurationInSeconds] DECIMAL (20, 4) NULL,
    [Rate]              DECIMAL (20, 8) NULL,
    [Amount]            DECIMAL (22, 6) NULL
);

