﻿CREATE TABLE [TOneWhS_Case].[CustomerCaseWorkGroup] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NULL,
    [CreatedTime] DATETIME         NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_CustomerCaseWorkGroup] PRIMARY KEY CLUSTERED ([ID] ASC)
);



