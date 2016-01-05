CREATE TABLE [common].[GenericConfiguration] (
    [ID]            INT           IDENTITY (1, 1) NOT NULL,
    [OwnerKey]      VARCHAR (50)  NULL,
    [TypeID]        INT           NULL,
    [ConfigDetails] VARCHAR (MAX) NULL,
    [timestamp]     ROWVERSION    NULL,
    CONSTRAINT [PK_GenericConfiguration] PRIMARY KEY CLUSTERED ([ID] ASC)
);

