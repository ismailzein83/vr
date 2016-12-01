CREATE TABLE [TOneWhS_Deal].[Deal] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Settings]    NVARCHAR (MAX) NOT NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_Deal_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Deal_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);





