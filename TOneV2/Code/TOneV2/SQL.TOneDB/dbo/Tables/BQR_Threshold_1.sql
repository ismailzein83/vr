CREATE TABLE [dbo].[BQR_Threshold] (
    [ID]                      INT           IDENTITY (1, 1) NOT NULL,
    [CriterionID]             INT           NOT NULL,
    [ThresholdType]           TINYINT       NOT NULL,
    [ThresholdValue]          VARCHAR (MAX) NOT NULL,
    [ActionType]              TINYINT       NOT NULL,
    [TargetServiceFlag]       SMALLINT      NULL,
    [timestamp]               ROWVERSION    NOT NULL,
    [ScheduledHoursBitVector] INT           NOT NULL,
    [EndEffectiveDate]        DATETIME      NULL,
    [IsActive]                CHAR (1)      NOT NULL,
    CONSTRAINT [PK_BQR_Threshold] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_BQR_Threshold_BQR_Criterion] FOREIGN KEY ([CriterionID]) REFERENCES [dbo].[BQR_Criterion] ([ID])
);

