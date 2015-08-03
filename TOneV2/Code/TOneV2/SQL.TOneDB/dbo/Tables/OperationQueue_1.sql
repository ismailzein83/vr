CREATE TABLE [dbo].[OperationQueue] (
    [Type]      CHAR (1) NOT NULL,
    [ID]        BIGINT   NOT NULL,
    [Operation] CHAR (1) NOT NULL,
    [Created]   DATETIME CONSTRAINT [DF_OperationQueue_Created] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_OperationQueue] PRIMARY KEY CLUSTERED ([Type] ASC, [ID] ASC)
);

