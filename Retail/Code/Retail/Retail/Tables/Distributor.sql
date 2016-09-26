CREATE TABLE [Retail].[Distributor] (
    [ID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NULL,
    [Type]        NVARCHAR (50)  NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [SourceId]    NVARCHAR (255) NULL,
    [CreatedDate] DATETIME       NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_Retail.Distributor] PRIMARY KEY CLUSTERED ([ID] ASC)
);

