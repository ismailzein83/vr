CREATE TYPE [VR_MobNet].[MobileCountryType] AS TABLE (
    [ID]               INT           NULL,
    [CountryID]        INT           NULL,
    [Settings]         VARCHAR (MAX) NULL,
    [CreatedBy]        INT           NULL,
    [CreatedTime]      DATETIME      NULL,
    [LastModifiedBy]   INT           NULL,
    [LastModifiedTime] DATETIME      NULL);

