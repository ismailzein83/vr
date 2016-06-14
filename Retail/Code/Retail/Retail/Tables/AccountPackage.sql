CREATE TABLE [Retail].[AccountPackage] (
    [ID]        INT        IDENTITY (1, 1) NOT NULL,
    [AccountID] INT        NOT NULL,
    [PackageID] INT        NOT NULL,
    [BED]       DATETIME   NOT NULL,
    [EED]       DATETIME   NULL,
    [timestamp] ROWVERSION NULL,
    CONSTRAINT [PK_AccountPackage] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UC_AccountPackage]
    ON [Retail].[AccountPackage]([AccountID] ASC, [PackageID] ASC);

