CREATE TYPE [RA].[MobileNetworkType] AS TABLE (
    [ID]               INT           NULL,
    [NetworkName]      VARCHAR (200) NULL,
    [Settings]         VARCHAR (MAX) NULL,
    [MobileCountryID]  INT           NULL,
    [CreatedBy]        INT           NULL,
    [CreatedTime]      DATETIME      NULL,
    [LastModifiedBy]   INT           NULL,
    [LastModifiedTime] DATETIME      NULL);

