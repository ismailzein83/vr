CREATE TABLE [dbo].[t1] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [ScheduleType]        TINYINT        NOT NULL,
    [Name]                NVARCHAR (255) NOT NULL,
    [Configuration]       NTEXT          NOT NULL,
    [IsEnabled]           CHAR (1)       NOT NULL,
    [TimeSpan]            VARCHAR (20)   NULL,
    [IsLastRunSuccessful] CHAR (1)       NULL,
    [LastRun]             SMALLDATETIME  NULL,
    [LastRunDuration]     VARCHAR (20)   NULL,
    [NextRun]             SMALLDATETIME  NULL,
    [timestamp]           ROWVERSION     NULL,
    [ExceptionString]     NTEXT          NULL,
    [GroupingExpression]  VARCHAR (512)  NULL
);

