CREATE TABLE [dbo].[ZebraSwitchConfiguration] (
    [ID]             INT          NOT NULL,
    [Req_ID]         INT          NULL,
    [IN_IP]          VARCHAR (15) NULL,
    [IsActive]       BIT          NULL,
    [IsApplied]      BIT          NULL,
    [ApplyingDate]   DATETIME     NULL,
    [ActivationDate] DATETIME     NULL,
    [CustomerID]     NVARCHAR (5) NULL,
    [Status]         TINYINT      NULL,
    [RequestDate]    DATETIME     NULL,
    CONSTRAINT [PK_ZebraSwitchConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC)
);

