CREATE TABLE [VR_Invoice].[Invoice] (
    [ID]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]              INT              NOT NULL,
    [InvoiceTypeID]       UNIQUEIDENTIFIER NOT NULL,
    [PartnerID]           VARCHAR (50)     NOT NULL,
    [SettlementInvoiceId] BIGINT           NULL,
    [InvoiceSettingID]    UNIQUEIDENTIFIER NULL,
    [SerialNumber]        NVARCHAR (255)   NOT NULL,
    [FromDate]            DATETIME         NOT NULL,
    [ToDate]              DATETIME         NOT NULL,
    [IssueDate]           DATE             NOT NULL,
    [DueDate]             DATE             NULL,
    [Details]             NVARCHAR (MAX)   NULL,
    [PaidDate]            DATETIME         NULL,
    [LockDate]            DATETIME         NULL,
    [SentDate]            DATETIME         NULL,
    [IsDeleted]           BIT              CONSTRAINT [DF_Invoice_IsDeleted] DEFAULT ((0)) NOT NULL,
    [Notes]               NVARCHAR (MAX)   NULL,
    [Settings]            NVARCHAR (MAX)   NULL,
    [SourceId]            NVARCHAR (50)    NULL,
    [IsDraft]             BIT              NULL,
    [IsAutomatic]         BIT              NULL,
    [CreatedTime]         DATETIME         CONSTRAINT [DF_Invoice_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED ([ID] ASC)
);


































GO
CREATE NONCLUSTERED INDEX [IX_Invoice_Type]
    ON [VR_Invoice].[Invoice]([InvoiceTypeID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Invoice_Partner]
    ON [VR_Invoice].[Invoice]([PartnerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Invoice_IssueDate]
    ON [VR_Invoice].[Invoice]([IssueDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Invoice_FromDate]
    ON [VR_Invoice].[Invoice]([FromDate] ASC);

