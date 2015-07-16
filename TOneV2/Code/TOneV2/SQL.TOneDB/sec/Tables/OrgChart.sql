﻿CREATE TABLE [sec].[OrgChart] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      VARCHAR (100)  NOT NULL,
    [Hierarchy] VARCHAR (1000) NOT NULL,
    CONSTRAINT [PK_OrgChart] PRIMARY KEY CLUSTERED ([Id] ASC)
);

