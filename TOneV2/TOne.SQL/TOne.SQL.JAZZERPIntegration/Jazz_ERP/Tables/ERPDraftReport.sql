CREATE TABLE [Jazz_ERP].[ERPDraftReport] (
    [ID]                 BIGINT           IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID]  BIGINT           NOT NULL,
    [ReportDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [SheetName]          NVARCHAR (255)   NOT NULL,
    [TransactionTypeID]  UNIQUEIDENTIFIER NULL,
    [IsTaxTransaction]   BIGINT           NULL,
    [CreatedTime]        DATETIME         CONSTRAINT [DF_ERPDraftReport_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]          ROWVERSION       NULL,
    [SwitchName]         VARCHAR (255)    NULL,
    [Direction]          INT              NOT NULL,
    [TransactionType]    VARCHAR (255)    NULL,
    [Processed]          BIT              NULL,
    CONSTRAINT [PK_ERPDraftReport] PRIMARY KEY CLUSTERED ([ID] ASC)
);



