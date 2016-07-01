﻿CREATE TYPE [TOneWhS_Analytics].[VariationReportTimePeriodType] AS TABLE (
    [PeriodIndex] INT      NOT NULL,
    [FromDate]    DATETIME NOT NULL,
    [ToDate]      DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([PeriodIndex] ASC));

