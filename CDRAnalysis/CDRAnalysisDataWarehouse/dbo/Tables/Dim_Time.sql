CREATE TABLE [dbo].[Dim_Time] (
    [DateInstance] DATETIME     NOT NULL,
    [Year]         INT          NULL,
    [Month]        INT          NULL,
    [Week]         INT          NULL,
    [Day]          INT          NULL,
    [Hour]         INT          NULL,
    [MonthName]    VARCHAR (50) NULL,
    [DayName]      VARCHAR (50) NULL,
    CONSTRAINT [PK_Dim_Time] PRIMARY KEY CLUSTERED ([DateInstance] ASC)
);

