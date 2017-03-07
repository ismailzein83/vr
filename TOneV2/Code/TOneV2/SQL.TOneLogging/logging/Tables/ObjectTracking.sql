﻿CREATE TABLE [logging].[ObjectTracking] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserID]            INT            NOT NULL,
    [LoggableEntityID]  INT            NOT NULL,
    [ObjectID]          VARCHAR (255)  NOT NULL,
    [ObjectDetails]     NVARCHAR (MAX) NULL,
    [ActionID]          INT            NOT NULL,
    [ActionDescription] NVARCHAR (MAX) NULL,
    [LogTime]           DATETIME       NOT NULL,
    [CreatedTime]       DATETIME       CONSTRAINT [DF_ObjectChangeTracking_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]         ROWVERSION     NULL,
    CONSTRAINT [PK_ObjectChangeTracking] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ObjectTracking_EntityAndObject]
    ON [logging].[ObjectTracking]([LoggableEntityID] ASC, [ObjectID] ASC);

