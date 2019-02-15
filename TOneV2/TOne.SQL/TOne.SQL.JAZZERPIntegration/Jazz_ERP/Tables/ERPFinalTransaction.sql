CREATE TABLE [Jazz_ERP].[ERPFinalTransaction] (
    [ID]                     BIGINT          IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID]      BIGINT          NOT NULL,
    [FromDate]               DATETIME        NOT NULL,
    [ToDate]                 DATETIME        NOT NULL,
    [TransactionCode]        VARCHAR (900)   NOT NULL,
    [TransactionDescription] NVARCHAR (MAX)  NOT NULL,
    [Credit]                 DECIMAL (26, 6) NOT NULL,
    [Debit]                  DECIMAL (26, 6) NOT NULL,
    [CreatedTime]            DATETIME        CONSTRAINT [DF_ERPFinalTransaction_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [timestamp]              ROWVERSION      NOT NULL,
    CONSTRAINT [PK_ERPFinalTransaction] PRIMARY KEY CLUSTERED ([ID] ASC)
);

