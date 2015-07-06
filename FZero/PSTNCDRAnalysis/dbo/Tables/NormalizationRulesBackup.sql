CREATE TABLE [dbo].[NormalizationRulesBackup] (
    [Id]                  INT             IDENTITY (1, 1) NOT NULL,
    [Prefix]              VARCHAR (2)     NULL,
    [CallLength]          INT             NULL,
    [In_TrunckName]       VARCHAR (5)     NULL,
    [Out_TrunckName]      VARCHAR (5)     NULL,
    [Durations]           NUMERIC (38, 4) NULL,
    [CallsCount]          INT             NULL,
    [Supplement]          VARCHAR (4)     NULL,
    [Ignore]              INT             NULL,
    [Party]               VARCHAR (4)     NULL,
    [SwitchId]            INT             NULL,
    [SwitchName]          VARCHAR (50)    NULL,
    [PrefixToAdd]         VARCHAR (6)     NULL,
    [SubstringStartIndex] INT             NULL,
    [SubstringLength]     INT             NULL,
    [SuffixToAdd]         VARCHAR (5)     NULL,
    [In_TrunckId]         INT             NULL,
    [Out_TrunckId]        INT             NULL
);

