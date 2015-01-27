CREATE TABLE [dbo].[rates] (
    [RateID]             BIGINT         IDENTITY (1, 1) NOT NULL,
    [PriceListID]        INT            NULL,
    [ZoneID]             INT            NULL,
    [Rate]               DECIMAL (9, 5) NULL,
    [OffPeakRate]        DECIMAL (9, 5) NULL,
    [WeekendRate]        DECIMAL (9, 5) NULL,
    [Change]             SMALLINT       NULL,
    [ServicesFlag]       SMALLINT       NULL,
    [BeginEffectiveDate] SMALLDATETIME  NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [IsEffective]        VARCHAR (1)    NOT NULL,
    [UserID]             INT            NULL,
    [timestamp]          ROWVERSION     NULL,
    [Notes]              NVARCHAR (255) NULL
);

