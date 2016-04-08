CREATE TABLE [dbo].[Dim_Schedules] (
    [Pk_ScheduleId] INT            NOT NULL,
    [Name]          NVARCHAR (255) NULL,
    CONSTRAINT [PK_Dim_Schedules] PRIMARY KEY CLUSTERED ([Pk_ScheduleId] ASC)
);

