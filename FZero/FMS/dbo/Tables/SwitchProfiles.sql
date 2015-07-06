CREATE TABLE [dbo].[SwitchProfiles] (
    [ID]                 INT        IDENTITY (1, 1) NOT NULL,
    [SwitchID]           FLOAT (53) NOT NULL,
    [TimeShiftinSeconds] INT        NOT NULL,
    [TimeDelayinMinutes] INT        NOT NULL,
    CONSTRAINT [PK_SwitchProfiles] PRIMARY KEY CLUSTERED ([ID] ASC)
);

