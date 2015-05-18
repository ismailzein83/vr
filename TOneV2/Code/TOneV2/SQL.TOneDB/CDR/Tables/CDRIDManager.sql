CREATE TABLE [CDR].[CDRIDManager] (
    [IsMain]      BIT    NOT NULL,
    [IsNegative]  BIT    NOT NULL,
    [LastTakenID] BIGINT NOT NULL,
    CONSTRAINT [PK_CDRIDManager] PRIMARY KEY CLUSTERED ([IsMain] ASC, [IsNegative] ASC)
);

