﻿CREATE TYPE [Mobile_EDR].[CorrelatedMobileCDRType] AS TABLE (
    [Id]                              BIGINT           NULL,
    [calling_record_type]             INT              NULL,
    [called_record_type]              INT              NULL,
    [calling_record_type_name]        NVARCHAR (100)   NULL,
    [called_record_type_name]         NVARCHAR (100)   NULL,
    [calling_number]                  VARCHAR (100)    NULL,
    [called_number]                   VARCHAR (100)    NULL,
    [setup_time]                      DATETIME         NULL,
    [connect_date_time]               DATETIME         NULL,
    [disconnect_date_time]            DATETIME         NULL,
    [call_duration]                   INT              NULL,
    [calling_imsi]                    VARCHAR (50)     NULL,
    [calling_imei]                    VARCHAR (50)     NULL,
    [called_imsi]                     VARCHAR (100)    NULL,
    [called_imei]                     VARCHAR (100)    NULL,
    [calling_switch_id]               INT              NULL,
    [called_switch_id]                INT              NULL,
    [calling_call_reference]          BIGINT           NULL,
    [called_call_reference]           BIGINT           NULL,
    [called_global_call_reference]    NVARCHAR (50)    NULL,
    [calling_global_call_reference]   NVARCHAR (50)    NULL,
    [calling_unique_identifier]       UNIQUEIDENTIFIER NULL,
    [called_unique_identifier]        UNIQUEIDENTIFIER NULL,
    [setup_timestamp]                 BIGINT           NULL,
    [connect_timestamp]               BIGINT           NULL,
    [disconnect_timestamp]            BIGINT           NULL,
    [calling_file_name]               NVARCHAR (200)   NULL,
    [called_file_name]                NVARCHAR (200)   NULL,
    [calling_first_ci]                NVARCHAR (255)   NULL,
    [calling_last_ci]                 NVARCHAR (255)   NULL,
    [called_first_ci]                 NVARCHAR (255)   NULL,
    [called_last_ci]                  NVARCHAR (255)   NULL,
    [intermediate_record_number]      INT              NULL,
    [intermediate_charging_indicator] INT              NULL);



