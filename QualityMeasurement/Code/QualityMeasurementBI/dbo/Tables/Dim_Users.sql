CREATE TABLE [dbo].[Dim_Users] (
    [Pk_UserId] INT            NOT NULL,
    [Name]      NVARCHAR (255) NULL,
    CONSTRAINT [PK_Dim_Users] PRIMARY KEY CLUSTERED ([Pk_UserId] ASC)
);

