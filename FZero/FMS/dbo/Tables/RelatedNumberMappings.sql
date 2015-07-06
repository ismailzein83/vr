CREATE TABLE [dbo].[RelatedNumberMappings] (
    [ID]                   INT           IDENTITY (1, 1) NOT NULL,
    [MobileOperatorID]     INT           NOT NULL,
    [ColumnName]           VARCHAR (100) NOT NULL,
    [MappedtoColumnNumber] INT           NOT NULL,
    CONSTRAINT [PK_RelatedNumberSourceMappings] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_RelatedNumberMappings_MobileOperators] FOREIGN KEY ([MobileOperatorID]) REFERENCES [dbo].[MobileOperators] ([ID]),
    CONSTRAINT [FK_RelatedNumberMappings_PredefinedColumnsforRelatedNumbers] FOREIGN KEY ([MappedtoColumnNumber]) REFERENCES [dbo].[PredefinedColumnsforRelatedNumbers] ([ID]),
    CONSTRAINT [FK_RelatedNumberSourceMappings_Sources] FOREIGN KEY ([MobileOperatorID]) REFERENCES [dbo].[Sources] ([ID])
);

