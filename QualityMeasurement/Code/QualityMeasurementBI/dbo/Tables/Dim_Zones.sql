CREATE TABLE [dbo].[Dim_Zones] (
    [Pk_ZoneId] INT            NOT NULL,
    [Name]      NVARCHAR (255) NULL,
    CONSTRAINT [PK_Dim_Zones] PRIMARY KEY CLUSTERED ([Pk_ZoneId] ASC)
);

