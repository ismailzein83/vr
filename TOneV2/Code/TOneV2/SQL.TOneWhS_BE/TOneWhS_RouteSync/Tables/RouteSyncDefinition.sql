CREATE TABLE [TOneWhS_RouteSync].[RouteSyncDefinition] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [Settings]    NVARCHAR (MAX) NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_RouteSyncDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_RouteSyncDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_RouteSyncDefinition_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



