CREATE TABLE [Retail].[AccountPackage] (
    [ID]                    BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountID]             BIGINT           NOT NULL,
    [PackageID]             INT              NOT NULL,
    [AccountBEDefinitionId] UNIQUEIDENTIFIER NOT NULL,
    [BED]                   DATETIME         NOT NULL,
    [EED]                   DATETIME         NULL,
    [timestamp]             ROWVERSION       NULL,
    [CreatedTime]           DATETIME         CONSTRAINT [DF_AccountPackage_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_AccountPackage] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_AccountPackage_Account] FOREIGN KEY ([AccountID]) REFERENCES [Retail].[Account] ([ID]),
    CONSTRAINT [FK_AccountPackage_Package] FOREIGN KEY ([PackageID]) REFERENCES [Retail].[Package] ([ID])
);










GO


