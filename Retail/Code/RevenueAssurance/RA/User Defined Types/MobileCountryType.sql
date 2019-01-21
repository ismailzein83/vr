CREATE TYPE [RA].[MobileCountryType] AS TABLE (
    [ID]               INT          NULL,
    [Code]             VARCHAR (10) NULL,
    [CountryID]        INT          NULL,
    [CreatedBy]        INT          NULL,
    [CreatedTime]      DATETIME     NULL,
    [LastModifiedBy]   INT          NULL,
    [LastModifiedTime] DATETIME     NULL);

