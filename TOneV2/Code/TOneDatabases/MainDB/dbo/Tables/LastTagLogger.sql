CREATE TABLE [dbo].[LastTagLogger] (
    [LastTag]    NVARCHAR (100) NULL,
    [LasttagId]  INT            IDENTITY (1, 1) NOT NULL,
    [ActionTime] NVARCHAR (50)  NULL,
    CONSTRAINT [PK_LastTagLogger_1] PRIMARY KEY CLUSTERED ([LasttagId] ASC)
);

