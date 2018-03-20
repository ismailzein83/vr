CREATE TABLE [sec].[User] (
    [ID]                    INT             IDENTITY (1, 1) NOT NULL,
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
    [DisabledTill]          DATETIME        NULL,
    [Settings]              NVARCHAR (MAX)  NULL,
    [ExtendedSettings]      NVARCHAR (MAX)  NULL,
    [IsSystemUser]          BIT             NULL,
    [CreatedTime]           DATETIME        CONSTRAINT [DF_User_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]             INT             NULL,
    [LastModifiedBy]        INT             NULL,
    [LastModifiedTime]      DATETIME        NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_User_Email] UNIQUE NONCLUSTERED ([Email] ASC)
);































