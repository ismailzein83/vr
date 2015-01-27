CREATE TABLE [dbo].[RoutingTask] (
    [TaskID]                 INT           IDENTITY (1, 1) NOT NULL,
    [IncludeBlockedZones]    BIT           NULL,
    [IncludeBlockRules]      BIT           NULL,
    [IncludeOverrides]       BIT           NULL,
    [IncludeSpecialRequests] BIT           NULL,
    [IncludeTOD]             BIT           NULL,
    [RebuildRouteOptionPool] BIT           NULL,
    [RebuildRoutePool]       BIT           NULL,
    [RebuildZoneRate]        BIT           NULL,
    [MaximumOptions]         INT           NULL,
    [TargetCustomers]        VARCHAR (MAX) NULL,
    [Targets]                VARCHAR (MAX) NULL,
    [TargetType]             TINYINT       NULL,
    [StartAt]                DATETIME      NULL,
    [IsComplete]             BIT           NULL,
    [IsFailed]               BIT           NULL,
    [IsStarted]              BIT           NULL,
    CONSTRAINT [PK_RoutingTask] PRIMARY KEY CLUSTERED ([TaskID] ASC)
);

