CREATE TABLE [dbo].[OperationType] (
    [ID]          INT          IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50) NOT NULL,
    [SPName]      VARCHAR (50) NULL,
    [StepOrder]   INT          NULL,
    [IsActive]    BIT          NULL,
    [Description] VARCHAR (50) NULL,
    CONSTRAINT [PK_ActionTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);

