CREATE TABLE [dbo].[BQR_CriterionOnZone] (
    [CriterionID] INT        NOT NULL,
    [ZoneID]      INT        NOT NULL,
    [timestamp]   ROWVERSION NOT NULL,
    [DS_ID_auto]  INT        IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [FK_BQR_CriterionOnZone_BQR_Criterion] FOREIGN KEY ([CriterionID]) REFERENCES [dbo].[BQR_Criterion] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [PK_BQR_CriterionOnZone] UNIQUE NONCLUSTERED ([CriterionID] ASC, [ZoneID] ASC)
);

