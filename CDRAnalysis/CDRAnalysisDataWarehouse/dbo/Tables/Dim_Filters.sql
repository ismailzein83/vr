CREATE TABLE [dbo].[Dim_Filters] (
    [Pk_FilterId] INT           NOT NULL,
    [Name]        VARCHAR (255) NULL,
    CONSTRAINT [PK_Dim_Filters] PRIMARY KEY CLUSTERED ([Pk_FilterId] ASC)
);

