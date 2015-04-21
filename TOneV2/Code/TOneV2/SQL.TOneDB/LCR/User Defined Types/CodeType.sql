CREATE TYPE [LCR].[CodeType] AS TABLE (
    [Code]            VARCHAR (20) NOT NULL,
    [IncludeSubCodes] BIT          NULL,
    PRIMARY KEY CLUSTERED ([Code] ASC));

