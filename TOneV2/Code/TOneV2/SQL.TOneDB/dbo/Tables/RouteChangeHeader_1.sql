CREATE TABLE [dbo].[RouteChangeHeader] (
    [ID]        INT           IDENTITY (1, 1) NOT NULL,
    [Reason]    VARCHAR (255) NOT NULL,
    [Created]   DATETIME      NOT NULL,
    [UserID]    INT           NOT NULL,
    [timestamp] ROWVERSION    NOT NULL,
    [IsEnded]   CHAR (1)      CONSTRAINT [DF_RouteChangeHeader_IsEnded] DEFAULT ('N') NOT NULL,
    CONSTRAINT [PK_RouteChangeHeader] PRIMARY KEY CLUSTERED ([ID] ASC)
);

