CREATE TABLE [common].[CacheRefreshHandle] (
    [CacheTypeName]  NVARCHAR (255) NOT NULL,
    [LastUpdateTime] DATETIME       NULL,
    [CreatedTime]    DATETIME       CONSTRAINT [DF_CacheRefreshHandle_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]      ROWVERSION     NULL,
    CONSTRAINT [PK_CacheRefreshHandle_1] PRIMARY KEY CLUSTERED ([CacheTypeName] ASC),
    CONSTRAINT [IX_CacheRefreshHandle_TypeName] UNIQUE NONCLUSTERED ([CacheTypeName] ASC)
);

