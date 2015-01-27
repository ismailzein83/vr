CREATE TABLE [dbo].[RouteQueue] (
    [CustomerID]         VARCHAR (5)    NOT NULL,
    [Code]               VARCHAR (15)   NOT NULL,
    [IncludeSubCodes]    CHAR (1)       CONSTRAINT [DF_RouteQueue_IncludeSubCodes] DEFAULT ('N') NOT NULL,
    [OurZoneID]          INT            NOT NULL,
    [RouteOptions]       VARCHAR (100)  NULL,
    [BlockedSuppliers]   VARCHAR (1024) NULL,
    [BeginEffectiveDate] SMALLDATETIME  NOT NULL,
    [EndEffectiveDate]   SMALLDATETIME  CONSTRAINT [DF_RouteQueue_EndEffectiveDate] DEFAULT ('2020-01-01') NOT NULL,
    [IsEffective]        AS             (case when getdate()>=[BeginEffectiveDate] AND getdate()<[EndEffectiveDate] then 'Y' else 'N' end),
    [Weight]             AS             ((case when [Code]<>'*ALL*' then (10) else (0) end+case when [OurZoneID]<>(0) then (1) else (0) end)+case when [CustomerID]<>'*ALL*' then (100) else (0) end) PERSISTED,
    [UserID]             INT            NULL,
    [Updated]            SMALLDATETIME  NULL,
    [ExcludedCodes]      VARCHAR (250)  NULL,
    [ApprovedUserID]     INT            NULL,
    [IsActive]           CHAR (1)       NULL,
    CONSTRAINT [PK_RouteQueue] PRIMARY KEY CLUSTERED ([CustomerID] ASC, [Code] ASC, [OurZoneID] ASC)
);

