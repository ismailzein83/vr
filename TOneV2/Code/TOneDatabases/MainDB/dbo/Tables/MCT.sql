CREATE TABLE [dbo].[MCT] (
    [ID]             INT           IDENTITY (1, 1) NOT NULL,
    [ParentID]       INT           NULL,
    [Title]          VARCHAR (100) NOT NULL,
    [MCTDescription] VARCHAR (250) NULL,
    [UserID]         INT           NULL,
    [Timestamp]      ROWVERSION    NOT NULL,
    CONSTRAINT [PK_MCT] PRIMARY KEY CLUSTERED ([ID] ASC)
);

