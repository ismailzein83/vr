﻿CREATE TABLE [dbo].[Set_CallType] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [Code]        INT          NULL,
    [Description] VARCHAR (50) NULL,
    CONSTRAINT [PK_Set_CallType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

