CREATE TABLE [TOneWhS_Routing].[RoutingDatabase] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [Title]         NVARCHAR (255) NOT NULL,
    [Type]          TINYINT        NOT NULL,
    [ProcessType]   TINYINT        NOT NULL,
    [EffectiveTime] DATETIME       NOT NULL,
    [IsReady]       BIT            NULL,
    [IsDeleted]     BIT            NULL,
    [CreatedTime]   DATETIME       CONSTRAINT [DF_RoutingDatabase_CreatedTime] DEFAULT (getdate()) NULL,
    [ReadyTime]     DATETIME       NULL,
    [DeletedTime]   DATETIME       NULL,
    [timestamp]     ROWVERSION     NULL,
    [Information]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_RoutingDatabase] PRIMARY KEY CLUSTERED ([ID] ASC)
);



