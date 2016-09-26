CREATE TYPE [Retail_EDR].[RingoPinType] AS TABLE (
    [Serial]         NVARCHAR (100) NULL,
    [Heading]        NVARCHAR (100) NULL,
    [Description]    NVARCHAR (100) NULL,
    [GenerationDate] DATETIME       NULL,
    [BurnDate]       DATETIME       NULL,
    [MSISDN]         NVARCHAR (100) NULL,
    [Token]          NVARCHAR (100) NULL,
    [AccountId]      BIGINT         NULL);



