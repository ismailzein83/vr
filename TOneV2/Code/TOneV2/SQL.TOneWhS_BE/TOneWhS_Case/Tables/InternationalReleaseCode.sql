CREATE TABLE [TOneWhS_Case].[InternationalReleaseCode] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NULL,
    [CreatedTime] DATETIME         NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_InternationalReleaseCode] PRIMARY KEY CLUSTERED ([ID] ASC)
);

