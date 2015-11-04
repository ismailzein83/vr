CREATE TABLE [common].[Currency] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Symbol]    NVARCHAR (10)  NOT NULL,
    [Name]      NVARCHAR (255) NOT NULL,
    [timestamp] ROWVERSION     NULL,
    CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED ([ID] ASC)
);

