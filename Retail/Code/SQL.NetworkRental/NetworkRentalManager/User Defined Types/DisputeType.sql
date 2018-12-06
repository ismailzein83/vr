CREATE TYPE [NetworkRentalManager].[DisputeType] AS TABLE (
    [ID]               BIGINT           NULL,
    [CustomerId]       BIGINT           NULL,
    [InvoiceReference] NVARCHAR (255)   NULL,
    [Notes]            NVARCHAR (1000)  NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [Attachments]      NVARCHAR (MAX)   NULL,
    [ReasonId]         UNIQUEIDENTIFIER NULL,
    [TypeId]           UNIQUEIDENTIFIER NULL,
    [StatusId]         UNIQUEIDENTIFIER NULL);

