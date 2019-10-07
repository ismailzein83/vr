CREATE TABLE [PATool].[ProjectProduct] (
    [ID]               INT        IDENTITY (1, 1) NOT NULL,
    [ProjectID]        INT        NULL,
    [ProductID]        INT        NULL,
    [CreatedTime]      DATETIME   NULL,
    [CreatedBy]        INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [timestamp]        ROWVERSION NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

