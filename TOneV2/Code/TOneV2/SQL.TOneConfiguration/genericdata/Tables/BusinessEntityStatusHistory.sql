CREATE TABLE [genericdata].[BusinessEntityStatusHistory] (
    [ID]                         BIGINT           IDENTITY (1, 1) NOT NULL,
    [BusinessEntityDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [BusinessEntityID]           VARCHAR (50)     NOT NULL,
    [FieldName]                  VARCHAR (50)     NULL,
    [StatusID]                   UNIQUEIDENTIFIER NOT NULL,
    [PreviousStatusID]           UNIQUEIDENTIFIER NULL,
    [StatusChangedDate]          DATETIME         NOT NULL,
    [IsDeleted]                  BIT              NULL,
    [CreatedTime]                DATETIME         CONSTRAINT [DF_BusinessEntityStatusHistory_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_BusinessEntityStatusHistory] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_BusinessEntityStatusHistory_BEDefId_FieldName_BEId_Time]
    ON [genericdata].[BusinessEntityStatusHistory]([BusinessEntityDefinitionID] ASC, [FieldName] ASC, [BusinessEntityID] ASC, [StatusChangedDate] DESC);

