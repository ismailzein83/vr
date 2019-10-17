CREATE TABLE [TelesEnterprise].[AccountEnterpriseDID] (
    [ID]                    BIGINT         IDENTITY (1, 1) NOT NULL,
    [AccountId]             BIGINT         NULL,
    [EnterpriseId]          VARCHAR (255)  NULL,
    [EnterpriseDescription] VARCHAR (1000) NULL,
    [SiteId]                VARCHAR (255)  NULL,
    [SiteDescription]       VARCHAR (1000) NULL,
    [Type]                  INT            NULL,
    [ScreenNumber]          NVARCHAR (255) NULL,
    [MaxCalls]              INT            NULL,
    CONSTRAINT [PK_AccountEnterpriseDID] PRIMARY KEY CLUSTERED ([ID] ASC)
);



