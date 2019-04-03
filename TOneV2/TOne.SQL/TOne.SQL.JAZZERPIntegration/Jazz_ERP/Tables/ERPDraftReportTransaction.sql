CREATE TABLE [Jazz_ERP].[ERPDraftReportTransaction] (
    [ID]                     BIGINT          IDENTITY (1, 1) NOT NULL,
    [ERPDraftReportID]       BIGINT          NOT NULL,
    [TransactionCode]        VARCHAR (900)   NOT NULL,
    [TransactionDescription] NVARCHAR (MAX)  NOT NULL,
    [Credit]                 DECIMAL (26, 6) NOT NULL,
    [Debit]                  DECIMAL (26, 6) NOT NULL,
    [CreatedTime]            DATETIME        CONSTRAINT [DF_ERPDraftReportTransaction_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]              ROWVERSION      NULL,
    [CarriersNames]          NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_ERPDraftReportTransaction] PRIMARY KEY CLUSTERED ([ID] ASC)
);



