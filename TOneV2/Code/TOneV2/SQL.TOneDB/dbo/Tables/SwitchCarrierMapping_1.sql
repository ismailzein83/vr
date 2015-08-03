CREATE TABLE [dbo].[SwitchCarrierMapping] (
    [SwitchID]         TINYINT       NULL,
    [CarrierAccountID] VARCHAR (10)  NULL,
    [Identifier]       VARCHAR (100) NULL,
    [IsIn]             CHAR (1)      NULL,
    [IsOut]            CHAR (1)      NULL,
    [InCDR]            CHAR (1)      NULL,
    [OutCDR]           CHAR (1)      NULL,
    [InRoute]          CHAR (1)      NULL,
    [OutRoute]         CHAR (1)      NULL,
    [Prefix]           VARCHAR (15)  NULL
);

