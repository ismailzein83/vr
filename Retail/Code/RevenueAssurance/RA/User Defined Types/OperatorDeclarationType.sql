CREATE TYPE [RA].[OperatorDeclarationType] AS TABLE (
    [ID]               BIGINT          NULL,
    [OperatorID]       BIGINT          NULL,
    [FromDate]         DATETIME        NULL,
    [ToDate]           DATETIME        NULL,
    [TotalVolumeInMin] DECIMAL (20, 4) NULL,
    [TotalRevenue]     DECIMAL (20, 4) NULL,
    [CurrencyID]       INT             NULL,
    [CreatedTime]      DATETIME        NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [LastModifiedBy]   INT             NULL);

