CREATE TABLE [dbo].[Dim_CallTestStatus] (
    [pk_CallTestStatusId] INT            NOT NULL,
    [CallTestStatus]      NVARCHAR (255) NULL,
    CONSTRAINT [PK_Dim_CallTestStatus] PRIMARY KEY CLUSTERED ([pk_CallTestStatusId] ASC)
);

