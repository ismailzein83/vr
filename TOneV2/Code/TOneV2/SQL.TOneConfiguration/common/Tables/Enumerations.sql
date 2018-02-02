CREATE TABLE [common].[Enumerations] (
    [ID]          INT           IDENTITY (1, 1) NOT NULL,
    [NameSpace]   VARCHAR (MAX) NULL,
    [Name]        VARCHAR (MAX) NULL,
    [Description] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_Enumerations] PRIMARY KEY CLUSTERED ([ID] ASC)
);



