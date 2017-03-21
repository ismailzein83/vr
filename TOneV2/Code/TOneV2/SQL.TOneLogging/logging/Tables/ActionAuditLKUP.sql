CREATE TABLE [logging].[ActionAuditLKUP] (
    [ID]          INT           IDENTITY (1, 1) NOT NULL,
    [Type]        INT           NOT NULL,
    [Name]        VARCHAR (255) NOT NULL,
    [CreatedTime] DATETIME      CONSTRAINT [DF_ActionAuditLKUP_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION    NULL,
    CONSTRAINT [PK_ActionAuditLKUP] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_ActionAuditLKUP_TypeName] UNIQUE NONCLUSTERED ([Type] ASC, [Name] ASC)
);




GO


