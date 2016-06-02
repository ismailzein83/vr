CREATE TABLE [dbo].[Dim_AccountStatus] (
    [Pk_AccountStatusId] INT          NOT NULL,
    [Name]               VARCHAR (50) NULL,
    CONSTRAINT [PK_Dim_AccountStatus] PRIMARY KEY CLUSTERED ([Pk_AccountStatusId] ASC)
);

