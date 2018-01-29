CREATE TABLE [logging].[ObjectTracking] (
    [ID]                   BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserID]               INT              NOT NULL,
    [LoggableEntityID]     UNIQUEIDENTIFIER NOT NULL,
    [ObjectID]             VARCHAR (255)    NOT NULL,
    [ObjectDetails]        NVARCHAR (MAX)   NULL,
    [ActionID]             INT              NOT NULL,
    [ActionDescription]    NVARCHAR (MAX)   NULL,
    [TechnicalInformation] NVARCHAR (MAX)   NULL,
    [LogTime]              DATETIME         NOT NULL,
    [ChangeInfo]           NVARCHAR (MAX)   NULL,
    [CreatedTime]          DATETIME         CONSTRAINT [DF_ObjectChangeTracking_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]            ROWVERSION       NULL,
    CONSTRAINT [PK_ObjectChangeTracking] PRIMARY KEY CLUSTERED ([ID] ASC)
);








GO


