CREATE TABLE [dbo].[Ext_DigitalkSummary] (
    [AgentId]     INT             NULL,
    [CallDate]    SMALLDATETIME   NULL,
    [Zone]        VARCHAR (200)   COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [ALeg]        NUMERIC (13, 4) NULL,
    [BLeg]        NUMERIC (13, 4) NULL,
    [Attempts]    INT             NULL,
    [Answered]    INT             NULL,
    [ASR]         NUMERIC (13, 4) NULL,
    [Charge]      NUMERIC (13, 4) NULL,
    [AgentPrefix] VARCHAR (50)    NULL
);

