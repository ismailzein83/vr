CREATE TABLE [dbo].[Dim_Countries] (
    [Pk_CountryId] INT            NOT NULL,
    [Name]         NVARCHAR (255) NULL,
    CONSTRAINT [PK_Dim_Countries] PRIMARY KEY CLUSTERED ([Pk_CountryId] ASC)
);

