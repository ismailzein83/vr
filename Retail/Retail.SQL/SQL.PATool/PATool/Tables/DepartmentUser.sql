CREATE TABLE [PATool].[DepartmentUser] (
    [ID]               INT        IDENTITY (1, 1) NOT NULL,
    [DepartmentID]     INT        NULL,
    [UserID]           INT        NULL,
    [CreatedTime]      DATETIME   NULL,
    [CreatedBy]        INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [timestamp]        ROWVERSION NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

