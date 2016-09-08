CREATE TABLE [Retail].[POS] (
    [ID]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (255) NULL,
    [Type]     NVARCHAR (50)  NULL,
    [Settings] NVARCHAR (MAX) NULL,
    [SourceId] NVARCHAR (255) NULL,
    CONSTRAINT [PK_Retail.POS] PRIMARY KEY CLUSTERED ([ID] ASC)
);

