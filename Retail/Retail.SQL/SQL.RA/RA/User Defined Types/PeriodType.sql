CREATE TYPE [RA].[PeriodType] AS TABLE (
    [ID]               INT             NULL,
    [FromDate]         DATETIME        NULL,
    [ToDate]           DATETIME        NULL,
    [Name]             NVARCHAR (1000) NULL,
    [CreatedBy]        INT             NULL,
    [CreatedTime]      DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [Repeat]           INT             NULL);

