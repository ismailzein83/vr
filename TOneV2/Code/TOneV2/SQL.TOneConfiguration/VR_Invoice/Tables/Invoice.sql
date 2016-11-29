CREATE TABLE [VR_Invoice].[Invoice] (
    [ID]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserId]        INT              NOT NULL,
    [InvoiceTypeID] UNIQUEIDENTIFIER NOT NULL,
    [PartnerID]     VARCHAR (50)     NOT NULL,
    [SerialNumber]  NVARCHAR (255)   NOT NULL,
    [FromDate]      DATE             NOT NULL,
    [ToDate]        DATE             NOT NULL,
    [IssueDate]     DATE             NOT NULL,
    [DueDate]       DATE             NULL,
    [Details]       NVARCHAR (MAX)   NULL,
    [PaidDate]      DATETIME         NULL,
    [LockDate]      DATETIME         NULL,
    [IsDeleted]     BIT              CONSTRAINT [DF_Invoice_IsDeleted] DEFAULT ((0)) NOT NULL,
    [Notes]         NVARCHAR (MAX)   NULL,
    [CreatedTime]   DATETIME         CONSTRAINT [DF_Invoice_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED ([ID] ASC)
);









