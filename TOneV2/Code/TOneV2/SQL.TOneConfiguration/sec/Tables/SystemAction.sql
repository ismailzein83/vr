CREATE TABLE [sec].[SystemAction] (
    [ID]                  INT             IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (100)  NOT NULL,
    [RequiredPermissions] NVARCHAR (1000) NULL,
    [timestamp]           ROWVERSION      NULL,
    CONSTRAINT [PK_sec.SystemAction] PRIMARY KEY CLUSTERED ([ID] ASC)
);



