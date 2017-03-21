CREATE TABLE [sec].[RequiredPermissionSet] (
    [ID]                       INT           IDENTITY (1, 1) NOT NULL,
    [Module]                   VARCHAR (255) NOT NULL,
    [RequiredPermissionString] VARCHAR (255) NOT NULL,
    [CreatedTime]              DATETIME      CONSTRAINT [DF_RequiredPermissionSet_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                ROWVERSION    NULL,
    CONSTRAINT [PK_RequiredPermissionSet] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_RequiredPermissionSet_ModuleReqPerm] UNIQUE NONCLUSTERED ([Module] ASC, [RequiredPermissionString] ASC)
);



