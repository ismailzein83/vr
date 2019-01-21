CREATE TABLE [RA].[NumberPrefix] (
    [ID]               BIGINT       IDENTITY (1, 1) NOT NULL,
    [Code]             VARCHAR (20) NULL,
    [BED]              DATETIME     NULL,
    [EED]              DATETIME     NULL,
    [MobileNetworkId]  INT          NULL,
    [CreatedBy]        INT          NULL,
    [CreatedTime]      DATETIME     NULL,
    [LastModifiedBy]   INT          NULL,
    [LastModifiedTime] DATETIME     NULL,
    [timestamp]        ROWVERSION   NOT NULL,
    CONSTRAINT [PK_NumberPrefix] PRIMARY KEY CLUSTERED ([ID] ASC)
);

