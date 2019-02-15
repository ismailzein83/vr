CREATE TABLE [Jazz_ERP].[ERPDraftReport] (
    [ID]                 BIGINT           IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID]  BIGINT           NOT NULL,
    [ReportDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [SheetName]          NVARCHAR (255)   NOT NULL,
    [TransactionTypeID]  UNIQUEIDENTIFIER NULL,
    [IsTaxTransaction]   BIGINT           NULL,
    [CreatedTime]        DATETIME         CONSTRAINT [DF_ERPDraftReport_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]          ROWVERSION       NULL,
    CONSTRAINT [PK_ERPDraftReport] PRIMARY KEY CLUSTERED ([ID] ASC)
);

