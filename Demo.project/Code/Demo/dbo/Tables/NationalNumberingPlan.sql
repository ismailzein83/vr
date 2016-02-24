CREATE TABLE [dbo].[NationalNumberingPlan] (
    [ID]         INT            IDENTITY (1, 1) NOT NULL,
    [OperatorID] INT            NOT NULL,
    [FromDate]   DATETIME       NOT NULL,
    [ToDate]     DATETIME       NULL,
    [Settings]   NVARCHAR (MAX) NULL,
    [timestamp]  ROWVERSION     NULL,
    CONSTRAINT [PK_NationalNumberingPlan] PRIMARY KEY CLUSTERED ([ID] ASC)
);

