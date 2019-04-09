CREATE TABLE [NetworkRentalManager].[DarkFiberSubscriptionOrder] (
    [ID]          BIGINT         NOT NULL,
    [Point1]      NVARCHAR (255) NULL,
    [Point2]      NVARCHAR (255) NULL,
    [Capacity]    BIGINT         NULL,
    [RatePlan]    BIGINT         NULL,
    [CreatedTime] DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

