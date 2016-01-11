CREATE TABLE [dbo].[Dim_NetworkType] (
    [Pk_NetTypeId] INT          NOT NULL,
    [Name]         VARCHAR (20) NULL,
    CONSTRAINT [PK_Dim_NetworkType] PRIMARY KEY CLUSTERED ([Pk_NetTypeId] ASC)
);

