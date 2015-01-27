CREATE TYPE [LCR].[DistinctCodeWithPossibleMatchType] AS TABLE (
    [DistinctCode]  VARCHAR (20) NOT NULL,
    [PossibleMatch] VARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([DistinctCode] ASC, [PossibleMatch] DESC));

