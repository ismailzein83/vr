CREATE TABLE [logging].[UserActionAudit] (
    [ID]         BIGINT        IDENTITY (1, 1) NOT NULL,
    [UserID]     INT           NOT NULL,
    [Module]     VARCHAR (50)  NOT NULL,
    [Controller] VARCHAR (50)  NOT NULL,
    [Action]     VARCHAR (100) NOT NULL,
    [LogTime]    DATETIME      CONSTRAINT [DF_UserActionAudit_LogTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_UserActionAudit] PRIMARY KEY CLUSTERED ([ID] ASC)
);



