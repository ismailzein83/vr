CREATE TABLE [dbo].[ServiceFlagMask] (
    [ServiceFlag] SMALLINT NOT NULL,
    [Mask]        SMALLINT NOT NULL,
    CONSTRAINT [PK_ServiceFlagMask] PRIMARY KEY CLUSTERED ([ServiceFlag] ASC, [Mask] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ServiceFlagMask]
    ON [dbo].[ServiceFlagMask]([Mask] ASC);

