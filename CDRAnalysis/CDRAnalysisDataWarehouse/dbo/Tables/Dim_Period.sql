CREATE TABLE [dbo].[Dim_Period] (
    [Pk_PeriodId] INT          NOT NULL,
    [Name]        VARCHAR (50) NULL,
    CONSTRAINT [PK_Dim_Period] PRIMARY KEY CLUSTERED ([Pk_PeriodId] ASC)
);

