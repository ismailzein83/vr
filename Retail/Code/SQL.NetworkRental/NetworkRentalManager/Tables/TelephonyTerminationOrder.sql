CREATE TABLE [NetworkRentalManager].[TelephonyTerminationOrder] (
    [ID]          BIGINT         NOT NULL,
    [PhoneNumber] NVARCHAR (255) NULL,
    [CreatedTime] DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

