﻿CREATE TABLE [dbo].[ReportingStatus] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ReportingStatus] PRIMARY KEY CLUSTERED ([ID] ASC)
);

