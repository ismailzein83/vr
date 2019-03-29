CREATE TABLE [NetworkRentalManager].[Order] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [OrderType]         UNIQUEIDENTIFIER NULL,
    [Customer]          BIGINT           NULL,
    [Status]            UNIQUEIDENTIFIER NULL,
    [ProcessInstanceId] BIGINT           NULL,
    [CreatedTime]       DATETIME         NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [CreatedBy]         INT              NULL,
    [LastModifiedBy]    INT              NULL,
    [Contract]          BIGINT           NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

