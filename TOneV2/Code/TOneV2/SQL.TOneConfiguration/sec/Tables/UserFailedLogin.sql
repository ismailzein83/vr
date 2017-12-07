CREATE TABLE [sec].[UserFailedLogin] (
    [ID]             BIGINT     IDENTITY (1, 1) NOT NULL,
    [UserID]         INT        NOT NULL,
    [FailedResultID] INT        NOT NULL,
    [CreatedTime]    DATETIME   CONSTRAINT [DF_UserFailedLogin_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]      ROWVERSION NULL,
    CONSTRAINT [PK_UserFailedLogin] PRIMARY KEY CLUSTERED ([ID] ASC)
);

