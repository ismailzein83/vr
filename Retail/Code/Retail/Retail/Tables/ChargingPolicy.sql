CREATE TABLE [Retail].[ChargingPolicy] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (255) NOT NULL,
    [ServiceTypeId] INT            NOT NULL,
    [Settings]      NVARCHAR (MAX) NOT NULL,
    [CreatedTime]   DATETIME       NULL,
    [timestamp]     ROWVERSION     NULL,
    CONSTRAINT [PK_ChargingPolicy] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ChargingPolicy_ServiceType] FOREIGN KEY ([ServiceTypeId]) REFERENCES [Retail].[ServiceType] ([ID]),
    CONSTRAINT [IX_ChargingPolicy_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

