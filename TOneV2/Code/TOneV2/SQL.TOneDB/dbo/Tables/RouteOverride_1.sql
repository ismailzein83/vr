CREATE TABLE [dbo].[RouteOverride] (
    [CustomerID]         VARCHAR (5)    NOT NULL,
    [Code]               VARCHAR (30)   NOT NULL,
    [IncludeSubCodes]    CHAR (1)       CONSTRAINT [DF_RouteOverride_IncludeSubCodes] DEFAULT ('N') NOT NULL,
    [OurZoneID]          INT            NOT NULL,
    [RouteOptions]       VARCHAR (100)  NULL,
    [BlockedSuppliers]   VARCHAR (1024) NULL,
    [BeginEffectiveDate] SMALLDATETIME  NOT NULL,
    [EndEffectiveDate]   DATETIME       NULL,
    [Weight]             AS             ((case when [Code]<>'*ALL*' then (10) else (0) end+case when [OurZoneID]<>(0) then (1) else (0) end)+case when [CustomerID]<>'*ALL*' then (100) else (0) end) PERSISTED,
    [UserID]             INT            NULL,
    [Updated]            SMALLDATETIME  NULL,
    [ExcludedCodes]      VARCHAR (250)  NULL,
    [Reason]             TEXT           NULL,
    [RouteOverrideID]    INT            IDENTITY (1, 1) NOT NULL,
    [timestamp]          ROWVERSION     NOT NULL,
    [IsEffective]        AS             (case when getdate()>=[BeginEffectiveDate] AND ([EndEffectiveDate] IS NULL OR getdate()<=[EndEffectiveDate]) then 'Y' else 'N' end),
    CONSTRAINT [PK_RouteOverride_1] PRIMARY KEY CLUSTERED ([RouteOverrideID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_RouteOverride_Begin]
    ON [dbo].[RouteOverride]([BeginEffectiveDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Zone]
    ON [dbo].[RouteOverride]([OurZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Code]
    ON [dbo].[RouteOverride]([Code] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Customer]
    ON [dbo].[RouteOverride]([CustomerID] ASC);

