CREATE TABLE [dbo].[RetailAgent] (
    [AgentID]   INT           IDENTITY (1, 1) NOT NULL,
    [Name]      VARCHAR (255) NOT NULL,
    [Prefix]    VARCHAR (255) NOT NULL,
    [ID]        VARCHAR (255) NULL,
    [Tag]       VARCHAR (255) NULL,
    [UserID]    INT           NULL,
    [timestamp] ROWVERSION    NOT NULL,
    CONSTRAINT [PK_RetailAgent] PRIMARY KEY CLUSTERED ([AgentID] ASC)
);

