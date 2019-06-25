CREATE TABLE [RetailBilling].[Service] (
    [ID]                 UNIQUEIDENTIFIER NOT NULL,
    [ContractTypeID]     UNIQUEIDENTIFIER NULL,
    [Name]               NVARCHAR (255)   NULL,
    [CreatedTime]        DATETIME         NULL,
    [CreatedBy]          INT              NULL,
    [LastModifiedTime]   DATETIME         NULL,
    [LastModifiedBy]     INT              NULL,
    [timestamp]          ROWVERSION       NULL,
    [DecreeId]           BIGINT           NULL,
    [ServiceCode]        NVARCHAR (255)   NULL,
    [ServiceDescription] NVARCHAR (1000)  NULL,
    [Translation]        NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK__Service__3214EC2703317E3D] PRIMARY KEY CLUSTERED ([ID] ASC)
);



