CREATE TABLE [dbo].[Dim_SuspicionLevel] (
    [Pk_SuspicionLevelId] INT          NOT NULL,
    [Name]                VARCHAR (50) NULL,
    CONSTRAINT [PK_Dim_SuspicionLevel] PRIMARY KEY CLUSTERED ([Pk_SuspicionLevelId] ASC)
);

