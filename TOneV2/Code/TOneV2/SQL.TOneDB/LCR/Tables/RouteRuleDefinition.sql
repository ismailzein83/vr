CREATE TABLE [LCR].[RouteRuleDefinition] (
    [RouteRuleId]          INT            IDENTITY (1, 1) NOT NULL,
    [CarrierAccountSet]    NVARCHAR (MAX) NULL,
    [CodeSet]              NVARCHAR (MAX) NULL,
    [ActionData]           NVARCHAR (MAX) NULL,
    [Type]                 INT            NOT NULL,
    [BeginEffectiveDate]   DATETIME       NOT NULL,
    [EndEffectiveDate]     DATETIME       NULL,
    [Reason]               NVARCHAR (MAX) NULL,
    [TimeExecutionSetting] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_RouteRuleDefinition] PRIMARY KEY CLUSTERED ([RouteRuleId] ASC)
);



