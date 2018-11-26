CREATE TABLE [NetworkRentalManager].[OrdersDefinition] (
    [ID]               BIGINT   IDENTITY (1, 1) NOT NULL,
    [OrderType]        INT      NULL,
    [StatusId]         INT      NULL,
    [AccountId]        BIGINT   NULL,
    [ProductId]        INT      NULL,
    [CreatedTime]      DATETIME CONSTRAINT [DF_OrdersDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT      NULL,
    [LastModifiedTime] DATETIME NULL,
    [LastModifiedBy]   INT      NULL,
    [SubContractorId]  BIGINT   NULL,
    CONSTRAINT [PK_OrdersDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);

