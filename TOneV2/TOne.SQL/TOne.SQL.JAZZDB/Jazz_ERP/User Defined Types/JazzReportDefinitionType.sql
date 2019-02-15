CREATE TYPE [Jazz_ERP].[JazzReportDefinitionType] AS TABLE (
    [ID]               UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Direction]        INT              NULL,
    [SwitchId]         INT              NULL,
    [TaxOption]        INT              NULL,
    [SplitRateValue]   DECIMAL (20, 8)  NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [IsEnabled]        BIT              NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [CreatedTime]      DATETIME         NULL);

