CREATE TABLE [dbo].[Dim_CallType] (
    [Pk_CallTypeId] INT          NOT NULL,
    [Name]          VARCHAR (50) NULL,
    CONSTRAINT [PK_Dim_CallType] PRIMARY KEY CLUSTERED ([Pk_CallTypeId] ASC)
);

