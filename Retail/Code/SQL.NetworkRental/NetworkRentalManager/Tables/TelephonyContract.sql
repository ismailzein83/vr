CREATE TABLE [NetworkRentalManager].[TelephonyContract] (
    [ID]          BIGINT         NOT NULL,
    [PhoneNumber] NVARCHAR (255) NULL,
    [RatePlan]    BIGINT         NULL,
    [CreatedTime] DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

