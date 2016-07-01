CREATE TABLE [Retail_BE].[StatusChargingSet] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Settings]    NVARCHAR (MAX) NOT NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_StatusChargingSet_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_StatusChargingSet] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_StatusChargingSet_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

