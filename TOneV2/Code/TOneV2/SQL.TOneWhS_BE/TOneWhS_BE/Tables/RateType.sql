CREATE TABLE [TOneWhS_BE].[RateType] (
    [ID]        INT           IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50) NOT NULL,
    [timestamp] ROWVERSION    NULL,
    CONSTRAINT [PK_RateType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_RateType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



