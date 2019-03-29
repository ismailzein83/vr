CREATE TABLE [NetworkRentalManager].[TelephonyComplaintType] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NULL,
    [CreatedTime] DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

