CREATE TABLE [NetworkRentalManager].[TelephonyServicesAdditionOrder] (
    [ID]          BIGINT         NOT NULL,
    [Services]    NVARCHAR (MAX) NULL,
    [CreatedTime] DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

