CREATE TABLE [RA_Retail].[PrepaidTransactionType] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_PrePaidTransactionType_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    [IsCredit]         BIT              NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

