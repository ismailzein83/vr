CREATE TABLE [Retail_BE].[FixedChargingPolicy] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Settings]    NVARCHAR (MAX) NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_FixedChargingPolicy_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_FixedChargingPolicy] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_FixedChargingPolicy_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

