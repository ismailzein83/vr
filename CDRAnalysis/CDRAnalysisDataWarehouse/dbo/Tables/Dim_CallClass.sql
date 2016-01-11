CREATE TABLE [dbo].[Dim_CallClass] (
    [Pk_CallClassId] INT          NOT NULL,
    [Name]           VARCHAR (50) NULL,
    CONSTRAINT [PK_Dim_CallClass] PRIMARY KEY CLUSTERED ([Pk_CallClassId] ASC)
);

