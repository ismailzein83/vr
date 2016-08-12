CREATE TABLE [VR_Invoice].[Invoice] (
    [ID]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [InvoiceTypeID] UNIQUEIDENTIFIER NOT NULL,
    [PartnerID]     VARCHAR (50)     NOT NULL,
    [SerialNumber]  NVARCHAR (255)   NOT NULL,
    [FromDate]      DATE             NOT NULL,
    [ToDate]        DATE             NOT NULL,
    [IssueDate]     DATE             NOT NULL,
    [DueDate]       DATE             NOT NULL,
    [Details]       NVARCHAR (MAX)   NULL,
    [CreatedTime]   DATETIME         CONSTRAINT [DF_Invoice_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED ([ID] ASC)
);



