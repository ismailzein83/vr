CREATE TABLE [sec].[User] (
    [ID]                    INT            IDENTITY (0, 1) NOT NULL,
    [Name]                  NVARCHAR (255) NOT NULL,
    [Password]              NVARCHAR (255) NULL,
    [Email]                 NVARCHAR (255) NULL,
    [TenantId]              INT            NULL,
    [Status]                INT            NULL,
    [LastLogin]             DATETIME       NULL,
    [Description]           NTEXT          NULL,
    [TempPassword]          NVARCHAR (255) NULL,
    [TempPasswordValidTill] DATETIME       NULL,
    [timestamp]             ROWVERSION     NULL,
    [EnabledTill]           DATETIME       NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([ID] ASC)
);













