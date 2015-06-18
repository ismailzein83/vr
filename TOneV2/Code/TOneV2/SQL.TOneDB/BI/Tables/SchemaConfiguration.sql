CREATE TABLE [BI].[SchemaConfiguration] (
    [ID]            INT             IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (50)   NULL,
    [Type]          NVARCHAR (50)   NULL,
    [Configuration] NVARCHAR (1000) NULL,
    CONSTRAINT [PK_MeasureDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);

