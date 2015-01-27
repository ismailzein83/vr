CREATE TABLE [dbo].[HeartBeat] (
    [ID]        INT        IDENTITY (1, 1) NOT NULL,
    [date]      DATETIME   CONSTRAINT [DF__HeartBeat__date__0A7E65A1] DEFAULT (getdate()) NULL,
    [TIMESTAMP] ROWVERSION NOT NULL,
    CONSTRAINT [PK__HeartBeat__098A4168] PRIMARY KEY CLUSTERED ([ID] ASC)
);

