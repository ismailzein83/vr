CREATE TABLE [dbo].[RouteOverride] (
    [CustomerID]         VARCHAR (5)    NOT NULL,
    [Code]               VARCHAR (30)   NOT NULL,
    [IncludeSubCodes]    CHAR (1)       CONSTRAINT [DF_RouteOverride_IncludeSubCodes] DEFAULT ('N') NOT NULL,
    [OurZoneID]          INT            NOT NULL,
    [RouteOptions]       VARCHAR (100)  NULL,
    [BlockedSuppliers]   VARCHAR (1024) NULL,
    [BeginEffectiveDate] SMALLDATETIME  NOT NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [IsEffective]        AS             (case when getdate()>=dateadd(minute,(-5),[BeginEffectiveDate]) AND ([EndEffectiveDate] IS NULL OR getdate()<=[EndEffectiveDate]) then 'Y' else 'N' end),
    [Weight]             AS             ((case when [Code]<>'*ALL*' then (10) else (0) end+case when [OurZoneID]<>(0) then (1) else (0) end)+case when [CustomerID]<>'*ALL*' then (100) else (0) end) PERSISTED,
    [UserID]             INT            NULL,
    [Updated]            SMALLDATETIME  NULL,
    [ExcludedCodes]      VARCHAR (250)  NULL,
    [Reason]             TEXT           NULL,
    CONSTRAINT [PK_RouteOverride] PRIMARY KEY CLUSTERED ([CustomerID] ASC, [Code] ASC, [OurZoneID] ASC)
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

