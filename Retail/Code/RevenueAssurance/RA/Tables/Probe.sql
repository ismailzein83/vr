CREATE TABLE [RA].[Probe] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK_Probe] PRIMARY KEY CLUSTERED ([ID] ASC)
);

