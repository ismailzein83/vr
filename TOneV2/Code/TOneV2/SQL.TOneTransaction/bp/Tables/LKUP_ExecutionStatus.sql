CREATE TABLE [bp].[LKUP_ExecutionStatus] (
    [ID]          INT          NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [IsOpened]    BIT          NULL,
    CONSTRAINT [PK_LKUP_ExecutionStatus] PRIMARY KEY CLUSTERED ([ID] ASC)
);



