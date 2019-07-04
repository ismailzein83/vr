CREATE TABLE [genericdata].[BusinessEntityHistoryStack] (
    [ID]                         BIGINT           IDENTITY (1, 1) NOT NULL,
    [BusinessEntityDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [BusinessEntityID]           VARCHAR (50)     NOT NULL,
    [FieldName]                  VARCHAR (50)     NULL,
    [StatusID]                   UNIQUEIDENTIFIER NOT NULL,
    [PreviousStatusID]           UNIQUEIDENTIFIER NULL,
    [StatusChangedDate]          DATETIME         NOT NULL,
    [IsDeleted]                  BIT              NULL,
    [CreatedTime]                DATETIME         CONSTRAINT [DF_BusinessEntityHistoryStack_CreatedTime] DEFAULT (getdate()) NULL,
    [MoreInfo]                   NVARCHAR (MAX)   NULL,
    [PreviousMoreInfo]           NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_BusinessEntityHistoryStack] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);

