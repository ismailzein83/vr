CREATE TYPE [TelesEnterprise].[AccountEnterpriseDID] AS TABLE (
    [AccountId]    BIGINT        NULL,
    [EnterpriseId] VARCHAR (255) NOT NULL,
    [ScreenNumber] VARCHAR (255) NOT NULL,
    [Type]         INT           NOT NULL,
    [MaxCalls]     INT           NOT NULL);

