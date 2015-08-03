CREATE TABLE [dbo].[Route] (
    [RouteID]                  INT          NOT NULL,
    [CustomerID]               VARCHAR (5)  NOT NULL,
    [ProfileID]                INT          NULL,
    [Code]                     VARCHAR (15) NULL,
    [OurZoneID]                INT          NULL,
    [OurActiveRate]            REAL         NULL,
    [OurNormalRate]            REAL         NULL,
    [OurOffPeakRate]           REAL         NULL,
    [OurWeekendRate]           REAL         NULL,
    [OurServicesFlag]          SMALLINT     NULL,
    [State]                    TINYINT      NOT NULL,
    [Updated]                  DATETIME     NULL,
    [IsToDAffected]            CHAR (1)     DEFAULT ('N') NOT NULL,
    [IsSpecialRequestAffected] CHAR (1)     DEFAULT ('N') NOT NULL,
    [IsBlockAffected]          CHAR (1)     DEFAULT ('N') NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_Route_Zone]
    ON [dbo].[Route]([OurZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Route_Updated]
    ON [dbo].[Route]([Updated] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_Route_Customer]
    ON [dbo].[Route]([CustomerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Route_Code]
    ON [dbo].[Route]([Code] ASC);


GO
CREATE CLUSTERED INDEX [PK_RouteID]
    ON [dbo].[Route]([RouteID] ASC);

