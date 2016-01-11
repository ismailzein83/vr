CREATE TABLE [dbo].[Dim_Strategy] (
    [Pk_StrategyId] INT          NOT NULL,
    [Name]          VARCHAR (50) NULL,
    [Type]          VARCHAR (50) NULL,
    [Kind]          VARCHAR (50) NULL,
    CONSTRAINT [PK_Dim_Strategy] PRIMARY KEY CLUSTERED ([Pk_StrategyId] ASC)
);

