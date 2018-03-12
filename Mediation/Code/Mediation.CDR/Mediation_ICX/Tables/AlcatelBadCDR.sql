CREATE TABLE [Mediation_ICX].[AlcatelBadCDR] (
    [Id]                BIGINT           NOT NULL,
    [DateOfCall]        DATE             NULL,
    [TimeOfCall]        TIME (3)         NULL,
    [DurationInSeconds] INT              NULL,
    [ANumber]           VARCHAR (50)     NULL,
    [BNumber]           VARCHAR (50)     NULL,
    [FileName]          VARCHAR (255)    NULL,
    [DataSourceId]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_AlcatelBadCDR] PRIMARY KEY CLUSTERED ([Id] ASC)
);





