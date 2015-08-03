CREATE TABLE [dbo].[PlaningRate] (
    [PlanningRateID]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [RatePlanID]         INT            NULL,
    [ZoneID]             INT            NULL,
    [Rate]               DECIMAL (9, 5) NULL,
    [OffPeakRate]        DECIMAL (9, 5) NULL,
    [WeekendRate]        DECIMAL (9, 5) NULL,
    [ServicesFlag]       SMALLINT       NULL,
    [BeginEffectiveDate] SMALLDATETIME  NULL,
    [UserID]             INT            NULL,
    [timestamp]          ROWVERSION     NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [Notes]              NVARCHAR (255) NULL,
    [Valid]              INT            NULL,
    CONSTRAINT [PK_PlaningRate] PRIMARY KEY CLUSTERED ([PlanningRateID] ASC),
    CONSTRAINT [FK_PlaningRate_RatePlan] FOREIGN KEY ([RatePlanID]) REFERENCES [dbo].[RatePlan] ([RatePlanID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PlaningRate]
    ON [dbo].[PlaningRate]([RatePlanID] ASC);

