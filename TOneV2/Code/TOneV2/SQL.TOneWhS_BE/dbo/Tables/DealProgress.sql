CREATE TABLE [dbo].[DealProgress] (
    [ID]                INT             IDENTITY (1, 1) NOT NULL,
    [Date]              DATE            NOT NULL,
    [IsSelling]         BIT             NOT NULL,
    [EstimatedDuration] DECIMAL (18, 4) NULL,
    [ReachedDuration]   DECIMAL (18, 4) NULL,
    [EstimatedAmount]   DECIMAL (18, 4) NULL,
    [ReachedAmount]     DECIMAL (18, 4) NULL,
    [timestamp]         ROWVERSION      NULL,
    CONSTRAINT [PK_DealProgress] PRIMARY KEY CLUSTERED ([ID] ASC)
);

