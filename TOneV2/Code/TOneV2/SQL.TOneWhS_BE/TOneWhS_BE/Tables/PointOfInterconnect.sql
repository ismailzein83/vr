CREATE TABLE [TOneWhS_BE].[PointOfInterconnect] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [SwitchId]         INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    [TrunkDetails]     NVARCHAR (MAX) NULL,
    [Name]             NVARCHAR (255) NULL,
    CONSTRAINT [PK_PointOfInterconnect] PRIMARY KEY CLUSTERED ([ID] ASC)
);

