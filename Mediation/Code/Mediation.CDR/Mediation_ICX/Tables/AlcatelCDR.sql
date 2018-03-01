CREATE TABLE [Mediation_ICX].[AlcatelCDR] (
    [Id]                BIGINT        NOT NULL,
    [DateOfCall]        DATE          NULL,
    [TimeOfCall]        TIME (3)      NULL,
    [DurationInSeconds] INT           NULL,
    [ANumber]           VARCHAR (50)  NULL,
    [BNumber]           VARCHAR (50)  NULL,
    [FileName]          VARCHAR (255) NULL,
    CONSTRAINT [PK_AlcatelCDR] PRIMARY KEY CLUSTERED ([Id] ASC)
);



