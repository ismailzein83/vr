CREATE TABLE [dbo].[SwitchCarrierMapping] (
    [SwitchID]         TINYINT       NULL,
    [CarrierAccountID] VARCHAR (10)  NULL,
    [Identifier]       VARCHAR (100) NULL,
    [IsIn]             CHAR (1)      NULL,
    [IsOut]            CHAR (1)      NULL,
    [ID]               INT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_SwitchCarrierMapping] PRIMARY KEY CLUSTERED ([ID] ASC)
);

