CREATE TABLE [dbo].[routeTemp] (
    [RouteID]                  INT          IDENTITY (1, 1) NOT NULL,
    [CustomerID]               VARCHAR (5)  NOT NULL,
    [Code]                     VARCHAR (15) NULL,
    [OurZoneID]                INT          NULL,
    [OurActiveRate]            REAL         NULL,
    [OurNormalRate]            REAL         NULL,
    [OurOffPeakRate]           REAL         NULL,
    [OurWeekendRate]           REAL         NULL,
    [OurServicesFlag]          SMALLINT     NULL,
    [State]                    TINYINT      NOT NULL,
    [Updated]                  DATETIME     NULL,
    [IsToDAffected]            CHAR (1)     NOT NULL,
    [IsSpecialRequestAffected] CHAR (1)     NOT NULL,
    [IsBlockAffected]          CHAR (1)     NOT NULL
);

