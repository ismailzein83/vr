CREATE TABLE [FraudAnalysis].[CDRDatabase] (
    [FromTime]          DATETIME       NOT NULL,
    [ToTime]            DATETIME       NULL,
    [Settings]          NVARCHAR (MAX) NULL,
    [IsReady]           BIT            NULL,
    [LockedByProcessID] INT            NULL,
    [CreatedTime]       DATETIME       NULL,
    [timestamp]         ROWVERSION     NULL,
    CONSTRAINT [PK_CDRDatabase] PRIMARY KEY CLUSTERED ([FromTime] ASC)
);

