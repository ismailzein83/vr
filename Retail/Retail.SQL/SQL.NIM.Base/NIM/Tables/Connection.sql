CREATE TABLE [NIM].[Connection] (
    [ID]               BIGINT   IDENTITY (1, 1) NOT NULL,
    [ModelID]          INT      NULL,
    [Port1]            BIGINT   NULL,
    [Port2]            BIGINT   NULL,
    [CreatedTime]      DATETIME NULL,
    [CreatedBy]        INT      NULL,
    [LastModifiedTime] DATETIME NULL,
    [LastModifiedBy]   INT      NULL,
    CONSTRAINT [PK__Connecti__3214EC271C1D2798] PRIMARY KEY CLUSTERED ([ID] ASC)
);

