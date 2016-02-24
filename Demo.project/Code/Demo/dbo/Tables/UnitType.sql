CREATE TABLE [dbo].[UnitType] (
    [ID]          INT            NOT NULL,
    [Description] NVARCHAR (255) NOT NULL,
    [timestamp]   ROWVERSION     NULL,
    CONSTRAINT [PK_UnitType] PRIMARY KEY CLUSTERED ([ID] ASC)
);

