CREATE TABLE [dbo].[DealDetail] (
    [ID]         INT             IDENTITY (1, 1) NOT NULL,
    [DealID]     INT             NULL,
    [FromMinute] BIGINT          NULL,
    [ToMinute]   BIGINT          NULL,
    [Rate]       DECIMAL (13, 6) NULL,
    [IsDeleted]  CHAR (1)        NULL,
    [timestamp]  ROWVERSION      NOT NULL,
    CONSTRAINT [PK_DealDetails] PRIMARY KEY CLUSTERED ([ID] ASC)
);

