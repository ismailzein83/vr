CREATE TABLE [common].[VRSequence] (
    [SequenceGroup]        VARCHAR (255)    NULL,
    [SequenceDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [SequenceKey]          NVARCHAR (255)   NOT NULL,
    [InitialValue]         BIGINT           NOT NULL,
    [LastValue]            BIGINT           NOT NULL,
    [CreatedTime]          DATETIME         CONSTRAINT [DF_InvoiceSequence_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]            ROWVERSION       NULL,
    CONSTRAINT [PK_InvoiceSequence] PRIMARY KEY CLUSTERED ([SequenceDefinitionID] ASC, [SequenceKey] ASC)
);

