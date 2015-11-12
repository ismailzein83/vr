CREATE TABLE [common].[IDManager] (
    [TypeID]      INT    NOT NULL,
    [LastTakenID] BIGINT NOT NULL,
    CONSTRAINT [PK_IDManager] PRIMARY KEY CLUSTERED ([TypeID] ASC)
);

