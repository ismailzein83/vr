CREATE TABLE [dbo].[ZGM_GroupedZones] (
    [ID]      INT IDENTITY (1, 1) NOT NULL,
    [GroupID] INT NOT NULL,
    [ZoneID]  INT NOT NULL,
    CONSTRAINT [PK_GroupedZones_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);

