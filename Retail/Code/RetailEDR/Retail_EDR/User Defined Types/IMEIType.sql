CREATE TYPE [Retail_EDR].[IMEIType] AS TABLE (
    [MSISDN]      NVARCHAR (100) NULL,
    [IMEI]        NVARCHAR (100) NULL,
    [CreatedDate] DATETIME       NULL,
    [AccountId]   BIGINT         NULL);

