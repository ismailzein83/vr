CREATE TABLE [NetworkRentalManager].[Contract] (
    [ID]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Customer]         BIGINT           NULL,
    [OrderId]          BIGINT           NULL,
    [Name]             NVARCHAR (255)   NULL,
    [ContractType]     UNIQUEIDENTIFIER NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedTime] DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    CONSTRAINT [PK__Contract__3214EC27534D60F1] PRIMARY KEY CLUSTERED ([ID] ASC)
);

