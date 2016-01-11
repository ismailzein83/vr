CREATE TABLE [dbo].[Dim_CaseStatus] (
    [Pk_CaseStatusId] INT          NOT NULL,
    [Name]            VARCHAR (50) NULL,
    CONSTRAINT [PK_Dim_CaseStatus] PRIMARY KEY CLUSTERED ([Pk_CaseStatusId] ASC)
);

