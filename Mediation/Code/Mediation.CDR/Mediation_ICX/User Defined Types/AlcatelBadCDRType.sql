CREATE TYPE [Mediation_ICX].[AlcatelBadCDRType] AS TABLE (
    [Id]                BIGINT           NULL,
    [DateOfCall]        DATE             NULL,
    [TimeOfCall]        TIME (3)         NULL,
    [DurationInSeconds] INT              NULL,
    [ANumber]           VARCHAR (50)     NULL,
    [BNumber]           VARCHAR (50)     NULL,
    [FileName]          VARCHAR (255)    NULL,
    [DataSourceId]      UNIQUEIDENTIFIER NULL);



