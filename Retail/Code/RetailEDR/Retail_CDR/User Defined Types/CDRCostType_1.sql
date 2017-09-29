CREATE TYPE [Retail_CDR].[CDRCostType] AS TABLE (
    [ID]                BIGINT          NULL,
    [SourceID]          VARCHAR (255)   NULL,
    [AttemptDateTime]   DATETIME        NULL,
    [CGPN]              VARCHAR (100)   NULL,
    [CDPN]              VARCHAR (100)   NULL,
    [SupplierName]      NVARCHAR (255)  NULL,
    [DurationInSeconds] DECIMAL (20, 4) NULL,
    [CurrencyId]        INT             NULL,
    [Rate]              DECIMAL (20, 8) NULL,
    [Amount]            DECIMAL (22, 6) NULL,
    [IsReRate]          BIT             NULL,
    [IsDeleted]         BIT             NULL,
    [FileName]          VARCHAR (255)   NULL,
    [ExtraFields]       NVARCHAR (MAX)  NULL,
    [ReceivedCGPN]      VARCHAR (100)   NULL,
    [ReceivedCDPN]      VARCHAR (100)   NULL);





