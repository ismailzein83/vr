CREATE TABLE [Com_CommonEntities].[NonWorkingDays] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [CalenderId]       INT            NULL,
    [Type]             INT            NULL,
    [Name]             NVARCHAR (MAX) NULL,
    [DayOfWeek]        INT            NULL,
    [Month]            INT            NULL,
    [Day]              INT            NULL,
    [Date]             DATETIME       NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL
);

