CREATE TABLE [common].[CountryArabic] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [CountryId]        INT            NOT NULL,
    [ArabicName]       NVARCHAR (255) NOT NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK__CountryA__10D1609F74E42A3D] PRIMARY KEY CLUSTERED ([Id] ASC)
);



