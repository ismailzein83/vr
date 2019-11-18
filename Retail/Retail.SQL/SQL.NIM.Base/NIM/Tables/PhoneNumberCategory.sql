CREATE TABLE [NIM].[PhoneNumberCategory] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    [OldID]            INT              NULL,
    CONSTRAINT [PK_PhoneNumberCategory] PRIMARY KEY CLUSTERED ([ID] ASC)
);

