CREATE TABLE [NetworkRentalManager].[Dispute] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [CustomerId]       BIGINT           NULL,
    [InvoiceReference] NVARCHAR (255)   NULL,
    [Notes]            NVARCHAR (1000)  NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_Dispute_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [Attachments]      NVARCHAR (MAX)   NULL,
    [ReasonId]         UNIQUEIDENTIFIER NULL,
    [TypeId]           UNIQUEIDENTIFIER NULL,
    [StatusId]         UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Dispute] PRIMARY KEY CLUSTERED ([ID] ASC)
);

