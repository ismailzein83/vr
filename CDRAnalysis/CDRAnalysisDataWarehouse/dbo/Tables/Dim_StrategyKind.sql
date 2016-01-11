CREATE TABLE [dbo].[Dim_StrategyKind] (
    [PK_KindId] INT          NOT NULL,
    [Name]      VARCHAR (50) NULL,
    CONSTRAINT [PK_Dim_StrategyKind] PRIMARY KEY CLUSTERED ([PK_KindId] ASC)
);

