CREATE TABLE [TOneWhS_BE].[TechnicalCode] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Code]             NVARCHAR (255) NULL,
    [ZoneID]           INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [timestamp]        ROWVERSION     NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

