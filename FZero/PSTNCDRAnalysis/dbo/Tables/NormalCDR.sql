CREATE TABLE [dbo].[NormalCDR] (
    [Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [ConnectDateTime]    DATETIME        NULL,
    [DisconnectDateTime] DATETIME        NULL,
    [DurationInSeconds]  NUMERIC (13, 4) NULL,
    [In_Trunk]           VARCHAR (5)     NULL,
    [Out_Trunk]          VARCHAR (5)     NULL,
    [In_Type]            VARCHAR (20)    NULL,
    [Out_Type]           VARCHAR (20)    NULL,
    [A_Temp]             VARCHAR (20)    NULL,
    [B_Temp]             VARCHAR (30)    NULL,
    [Switch]             VARCHAR (30)    NULL,
    [IsRepeated]         INT             NULL,
    [SwitchId]           INT             NULL,
    [SwitchRecordId]     INT             NULL,
    CONSTRAINT [PK_normalCDR] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 80, STATISTICS_NORECOMPUTE = ON)
);


GO
CREATE NONCLUSTERED INDEX [IX_connect_date_time]
    ON [dbo].[NormalCDR]([ConnectDateTime] ASC) WITH (FILLFACTOR = 80, STATISTICS_NORECOMPUTE = ON);

