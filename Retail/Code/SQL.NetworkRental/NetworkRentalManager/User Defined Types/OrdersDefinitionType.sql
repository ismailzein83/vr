CREATE TYPE [NetworkRentalManager].[OrdersDefinitionType] AS TABLE (
    [ID]               BIGINT   NULL,
    [OrderType]        INT      NULL,
    [StatusId]         INT      NULL,
    [AccountId]        BIGINT   NULL,
    [ProductId]        INT      NULL,
    [CreatedTime]      DATETIME NULL,
    [CreatedBy]        INT      NULL,
    [LastModifiedTime] DATETIME NULL,
    [LastModifiedBy]   INT      NULL,
    [SubContractorId]  BIGINT   NULL);

