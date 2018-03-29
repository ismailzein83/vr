CREATE TYPE [common].[VRBulkActionDraftTable] AS TABLE (
    [BulkActionDraftIdentifier] UNIQUEIDENTIFIER NOT NULL,
    [ItemId]                    VARCHAR (255)    NOT NULL);

