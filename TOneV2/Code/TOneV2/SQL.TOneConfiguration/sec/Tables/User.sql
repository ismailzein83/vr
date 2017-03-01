CREATE TABLE [sec].[User] (
    [ID]                    INT             IDENTITY (0, 1) NOT NULL,
    [Name]                  NVARCHAR (255)  NOT NULL,
    [Password]              NVARCHAR (255)  NULL,
    [Email]                 NVARCHAR (255)  NULL,
    [TenantId]              INT             NULL,
    [LastLogin]             DATETIME        NULL,
    [Description]           NVARCHAR (1000) NULL,
    [TempPassword]          NVARCHAR (255)  NULL,
    [TempPasswordValidTill] DATETIME        NULL,
    [timestamp]             ROWVERSION      NULL,
    [EnabledTill]           DATETIME        NULL,
    [ExtendedSettings]      NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([ID] ASC)
);



















