CREATE TABLE [RA].[MobileCountry] (
    [ID]               INT          IDENTITY (1, 1) NOT NULL,
    [Code]             VARCHAR (10) NULL,
    [CountryID]        INT          NULL,
    [CreatedBy]        INT          NULL,
    [CreatedTime]      DATETIME     NULL,
    [LastModifiedBy]   INT          NULL,
    [LastModifiedTime] DATETIME     NULL,
    [timestamp]        ROWVERSION   NULL,
    CONSTRAINT [PK_MobileCountry] PRIMARY KEY CLUSTERED ([ID] ASC)
);

