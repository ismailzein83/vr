CREATE TABLE [dbo].[Dim_User] (
    [Pk_UserId] INT          NOT NULL,
    [Name]      VARCHAR (50) NULL,
    CONSTRAINT [PK_Dim_Users] PRIMARY KEY CLUSTERED ([Pk_UserId] ASC)
);

