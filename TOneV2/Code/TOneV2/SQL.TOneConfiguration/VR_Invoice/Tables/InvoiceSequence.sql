CREATE TABLE [VR_Invoice].[InvoiceSequence] (
    [SequenceGroup] VARCHAR (255)    NULL,
    [InvoiceTypeID] UNIQUEIDENTIFIER NOT NULL,
    [SequenceKey]   NVARCHAR (255)   NOT NULL,
    [InitialValue]  BIGINT           NOT NULL,
    [LastValue]     BIGINT           NOT NULL,
    [CreatedTime]   DATETIME         CONSTRAINT [DF_InvoiceSequence_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]     ROWVERSION       NULL,
    CONSTRAINT [PK_InvoiceSequence] PRIMARY KEY CLUSTERED ([InvoiceTypeID] ASC, [SequenceKey] ASC)
);



