CREATE TABLE [dbo].[TempRoute] (
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

