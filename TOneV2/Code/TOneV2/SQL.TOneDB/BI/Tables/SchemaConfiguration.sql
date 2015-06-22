CREATE TABLE [BI].[SchemaConfiguration] (
    [ID]            INT             IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (50)   NOT NULL,
    [Type]          INT             NOT NULL,
    [Configuration] NVARCHAR (1000) NOT NULL,
    CONSTRAINT [PK_MeasureDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);



