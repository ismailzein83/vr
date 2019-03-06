CREATE TABLE [Jazz_ERP].[JazzReportDefinition] (
    [ID]                UNIQUEIDENTIFIER NOT NULL,
    [Name]              NVARCHAR (255)   NULL,
    [Direction]         INT              NULL,
    [SwitchId]          INT              NULL,
    [LastModifiedBy]    INT              NULL,
    [timestamp]         ROWVERSION       NULL,
    [IsEnabled]         BIT              NULL,
    [Settings]          NVARCHAR (MAX)   NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [CreatedTime]       DATETIME         CONSTRAINT [DF_JazzReportDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]         INT              NULL,
    [TaxOption]         INT              NULL,
    [AmountMeasureType] INT              NULL,
    [AmountType]        INT              NULL,
    [SplitRateValue]    DECIMAL (20, 8)  NULL,
    [CurrencyId]        INT              NULL,
    CONSTRAINT [PK_JazzReportDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);



