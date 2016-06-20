CREATE TABLE [Retail].[AccountService] (
    [ID]                      BIGINT         IDENTITY (1, 1) NOT NULL,
    [AccountId]               BIGINT         NULL,
    [ServiceTypeID]           INT            NULL,
    [ServiceChargingPolicyID] INT            NULL,
    [Settings]                NVARCHAR (MAX) NULL,
    [CreatedTime]             DATETIME       CONSTRAINT [DF_AccountService_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]               ROWVERSION     NULL,
    CONSTRAINT [PK_AccountService] PRIMARY KEY CLUSTERED ([ID] ASC)
);

