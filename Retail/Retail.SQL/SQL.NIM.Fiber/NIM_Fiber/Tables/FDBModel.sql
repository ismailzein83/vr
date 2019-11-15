CREATE TABLE [NIM_Fiber].[FDBModel] (
    [ID]               INT        NOT NULL,
    [LastModifiedTime] DATETIME   NULL,
    [timestamp]        ROWVERSION NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

