CREATE TABLE [bp].[BPTaskAssignedUser] (
    [ID]     BIGINT IDENTITY (1, 1) NOT NULL,
    [TaskID] BIGINT NULL,
    [UserID] BIGINT NULL,
    CONSTRAINT [PK_BPTaskAssignees] PRIMARY KEY CLUSTERED ([ID] ASC)
);

