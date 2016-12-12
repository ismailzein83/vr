﻿CREATE TABLE [logging].[UserActionAudit] (
    [ID]             BIGINT        IDENTITY (1, 1) NOT NULL,
    [UserID]         INT           NULL,
    [ModuleName]     VARCHAR (50)  NOT NULL,
    [ControllerName] VARCHAR (50)  NOT NULL,
    [ActionName]     VARCHAR (100) NOT NULL,
    [LogTime]        DATETIME      CONSTRAINT [DF_UserActionAudit_LogTime] DEFAULT (getdate()) NULL,
    [BaseUrl]        VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_UserActionAudit] PRIMARY KEY CLUSTERED ([ID] ASC)
);





