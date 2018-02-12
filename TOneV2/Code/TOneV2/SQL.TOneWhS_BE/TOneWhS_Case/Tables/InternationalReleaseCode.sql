CREATE TABLE [TOneWhS_Case].[InternationalReleaseCode] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [CreatedTime]      DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    CONSTRAINT [PK_InternationalReleaseCode] PRIMARY KEY CLUSTERED ([ID] ASC)
);



