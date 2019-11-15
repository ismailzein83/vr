CREATE TABLE [NIM].[NodePart] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Number]           NVARCHAR (450) NULL,
    [NodeID]           BIGINT         NULL,
    [ModelID]          INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [ParentPartID]     BIGINT         NULL,
    [Notes]            NVARCHAR (MAX) NULL,
    CONSTRAINT [PK__NodePart__3214EC2737FA4C37] PRIMARY KEY CLUSTERED ([ID] ASC)
);

