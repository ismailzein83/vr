CREATE TABLE [dbo].[OriginationNetworks] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_OriginationNetworks] PRIMARY KEY CLUSTERED ([ID] ASC)
);

