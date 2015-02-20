CREATE TABLE [LCR].[RoutingDatabase] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [Title]         NVARCHAR (255) NOT NULL,
    [Type]          INT            NOT NULL,
    [EffectiveTime] DATETIME       NOT NULL,
    [IsReady]       BIT            NULL,
    [IsDeleted]     BIT            NULL,
    [CreatedTime]   DATETIME       CONSTRAINT [DF_RoutingDatabase_CreatedTime] DEFAULT (getdate()) NULL,
    [ReadyTime]     DATETIME       NULL,
    [DeletedTime]   DATETIME       NULL,
    CONSTRAINT [PK_RoutingDatabase] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_RoutingDatabase_IsReady]
    ON [LCR].[RoutingDatabase]([IsReady] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RoutingDatabase_IsDeleted]
    ON [LCR].[RoutingDatabase]([IsDeleted] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RoutingDatabase_EffectiveTime]
    ON [LCR].[RoutingDatabase]([EffectiveTime] DESC);

