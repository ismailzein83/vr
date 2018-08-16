CREATE TABLE [bp].[BPTimeSubscription] (
    [ID]                BIGINT        IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID] BIGINT        NOT NULL,
    [Bookmark]          VARCHAR (100) NOT NULL,
    [DueTime]           DATETIME      NOT NULL,
    [Payload]           VARCHAR (MAX) NULL,
    [CreatedTime]       DATETIME      CONSTRAINT [DF_BPTimeSubscription_CreatedTime] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_BPTimeSubscription] PRIMARY KEY CLUSTERED ([ID] ASC)
);



