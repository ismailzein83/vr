CREATE TABLE [NIM].[NodePort] (
    [ID]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [Number]              NVARCHAR (450)   NULL,
    [ModelID]             INT              NULL,
    [NodeID]              BIGINT           NULL,
    [PartID]              BIGINT           NULL,
    [Status]              UNIQUEIDENTIFIER NULL,
    [CreatedTime]         DATETIME         NULL,
    [CreatedBy]           INT              NULL,
    [LastModifiedTime]    DATETIME         NULL,
    [LastModifiedBy]      INT              NULL,
    [Notes]               NVARCHAR (MAX)   NULL,
    [ConnectionDirection] INT              NULL,
    CONSTRAINT [PK__NodePort__3214EC274183B671] PRIMARY KEY CLUSTERED ([ID] ASC)
);

