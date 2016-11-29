CREATE TABLE [Retail].[AccountPackage] (
    [ID]          INT        IDENTITY (1, 1) NOT NULL,
    [AccountID]   INT        NOT NULL,
    [PackageID]   INT        NOT NULL,
    [BED]         DATETIME   NOT NULL,
    [EED]         DATETIME   NULL,
    [timestamp]   ROWVERSION NULL,
    [CreatedTime] DATETIME   CONSTRAINT [DF_AccountPackage_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_AccountPackage] PRIMARY KEY CLUSTERED ([ID] ASC)
);




GO


