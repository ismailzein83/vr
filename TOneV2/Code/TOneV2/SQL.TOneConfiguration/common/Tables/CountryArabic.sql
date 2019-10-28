CREATE TABLE [common].[CountryArabic] (
    [CountryId]        INT            NOT NULL,
    [ArabicName]       NVARCHAR (255) NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    PRIMARY KEY CLUSTERED ([CountryId] ASC)
);

