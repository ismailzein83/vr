CREATE TABLE [dbo].[SwitchTruncksold] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [SwitchId]    INT            NOT NULL,
    [TrunckId]    INT            NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [FullName]    NVARCHAR (250) NOT NULL,
    [DirectionId] TINYINT        NOT NULL,
    CONSTRAINT [PK_SwitchTruncks] PRIMARY KEY CLUSTERED ([Id] ASC)
);

