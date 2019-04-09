CREATE TABLE [NetworkRentalManager].[DarkFiberContract] (
    [ID]          BIGINT   NOT NULL,
    [Capacity]    INT      NULL,
    [RatePlan]    BIGINT   NULL,
    [CreatedTime] DATETIME NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

