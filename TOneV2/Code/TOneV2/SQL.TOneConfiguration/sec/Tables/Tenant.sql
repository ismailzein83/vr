CREATE TABLE [sec].[Tenant] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NULL,
    [ParentTenantID]   INT            NULL,
    [Settings]         NVARCHAR (MAX) NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_Tenant_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION     NULL,
    [LastModifiedTime] DATETIME       CONSTRAINT [DF_Tenant_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Tenant] PRIMARY KEY CLUSTERED ([ID] ASC)
);



