CREATE TABLE [dbo].[RoutingApprovalConfiguration] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Description]      NVARCHAR (50)  NULL,
    [ConnectionString] NVARCHAR (250) NULL,
    [RouteQueueTable]  NVARCHAR (50)  NULL,
    [SwitchID]         INT            NULL,
    [UserID]           INT            NULL,
    [SwitchNamespace]  NVARCHAR (200) NULL,
    CONSTRAINT [PK_RoutingApprovalConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC)
);

