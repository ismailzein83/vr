CREATE TABLE [sec].[SystemAction] (
    [ID]                  INT             IDENTITY (1, 1) NOT NULL,
    [Name]                VARCHAR (900)   NOT NULL,
    [RequiredPermissions] NVARCHAR (1000) NULL,
    [timestamp]           ROWVERSION      NULL,
    [LastModifiedTime]    DATETIME        CONSTRAINT [DF_SystemAction_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_sec.SystemAction] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UK_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);







