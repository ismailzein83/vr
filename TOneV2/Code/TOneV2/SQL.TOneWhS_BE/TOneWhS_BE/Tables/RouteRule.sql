CREATE TABLE [TOneWhS_BE].[RouteRule] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Criteria]         NVARCHAR (MAX) NOT NULL,
    [TypeConfigID]     INT            NOT NULL,
    [RuleSettings]     NVARCHAR (MAX) NULL,
    [Description]      NVARCHAR (MAX) NULL,
    [BED]              DATETIME       NOT NULL,
    [EED]              DATETIME       NULL,
    [ScheduleSettings] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_RouteRule] PRIMARY KEY CLUSTERED ([ID] ASC)
);

