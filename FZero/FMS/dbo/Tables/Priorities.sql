﻿CREATE TABLE [dbo].[Priorities] (
    [ID]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Priorities] PRIMARY KEY CLUSTERED ([ID] ASC)
);

