CREATE TABLE [dbo].[OperatorConfiguration] (
    [ID]                     INT            IDENTITY (1, 1) NOT NULL,
    [OperatorID]             INT            NOT NULL,
    [Volume]                 INT            NOT NULL,
    [CDRDirection]           INT            NOT NULL,
    [Percentage]             FLOAT (53)     NULL,
    [Amount]                 FLOAT (53)     NULL,
    [Currency]               INT            NULL,
    [FromDate]               DATETIME       NOT NULL,
    [ToDate]                 DATETIME       NULL,
    [Notes]                  NVARCHAR (MAX) NULL,
    [ServiceSubTypeSettings] NVARCHAR (MAX) NULL,
    [DestinationGroup]       INT            NULL,
    [InterconnectOperator]   INT            NULL,
    [timestamp]              ROWVERSION     NULL,
    CONSTRAINT [PK_OperatorConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC)
);



