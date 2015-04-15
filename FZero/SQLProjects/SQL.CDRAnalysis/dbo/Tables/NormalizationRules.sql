CREATE TABLE [dbo].[NormalizationRules] (
    [Id]                  INT             IDENTITY (1, 1) NOT NULL,
    [Prefix]              VARCHAR (3)     NULL,
    [CallLength]          INT             NULL,
    [Durations]           NUMERIC (38, 4) NULL,
    [CallsCount]          INT             NULL,
    [Ignore]              INT             NULL,
    [Party]               VARCHAR (10)    NULL,
    [SwitchId]            INT             NULL,
    [SwitchName]          VARCHAR (50)    NULL,
    [PrefixToAdd]         VARCHAR (6)     NULL,
    [SubstringStartIndex] INT             NULL,
    [SubstringLength]     INT             NULL,
    [SuffixToAdd]         VARCHAR (5)     NULL,
    CONSTRAINT [PK_NormalizationRules] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NormalizationRules_SwitchProfiles] FOREIGN KEY ([SwitchId]) REFERENCES [dbo].[SwitchProfiles] ([Id])
);

