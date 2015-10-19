CREATE TABLE [TOneWhS_BE].[Currency] (
    [ID]       INT            IDENTITY (1, 1) NOT NULL,
    [Symbol]   NVARCHAR (10)  NOT NULL,
    [Name]     NVARCHAR (255) NOT NULL,
    [LastRate] FLOAT (53)     NULL,
    CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED ([ID] ASC)
);

