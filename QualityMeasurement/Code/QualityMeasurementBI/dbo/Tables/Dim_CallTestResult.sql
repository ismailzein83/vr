CREATE TABLE [dbo].[Dim_CallTestResult] (
    [pk_CallTestResultId] INT            NOT NULL,
    [CallTestResult]      NVARCHAR (255) NULL,
    CONSTRAINT [PK_Dim_CallTestResult] PRIMARY KEY CLUSTERED ([pk_CallTestResultId] ASC)
);

