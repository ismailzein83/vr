CREATE TYPE [TelesEnterprise].[AccountEnterpriseDID] AS TABLE (
    [AccountId]             BIGINT         NULL,
    [EnterpriseId]          VARCHAR (255)  NOT NULL,
    [EnterpriseDescription] VARCHAR (1000) NULL,
    [SiteId]                VARCHAR (255)  NULL,
    [SiteDescription]       VARCHAR (1000) NULL,
    [ScreenNumber]          VARCHAR (255)  NOT NULL,
    [Type]                  INT            NOT NULL,
    [MaxCalls]              INT            NOT NULL);



