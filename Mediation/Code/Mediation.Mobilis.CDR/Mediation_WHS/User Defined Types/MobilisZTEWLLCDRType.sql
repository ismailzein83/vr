﻿CREATE TYPE [Mediation_WHS].[MobilisZTEWLLCDRType] AS TABLE (
    [Id]                      BIGINT           NULL,
    [LACOfSubscriber]         VARCHAR (10)     NULL,
    [CIOfSubscriber]          VARCHAR (10)     NULL,
    [TicketType]              VARCHAR (5)      NULL,
    [CityCodeOfCallingNumber] VARCHAR (15)     NULL,
    [CallingNumber]           VARCHAR (15)     NULL,
    [CallDate]                DATE             NULL,
    [CallTime]                TIME (3)         NULL,
    [CalledNumber]            VARCHAR (25)     NULL,
    [Duration]                INT              NULL,
    [Pulse]                   VARCHAR (10)     NULL,
    [CallType]                VARCHAR (5)      NULL,
    [SPS800]                  VARCHAR (5)      NULL,
    [Filler]                  VARCHAR (15)     NULL,
    [FileName]                VARCHAR (255)    NULL,
    [DataSourceId]            UNIQUEIDENTIFIER NULL);

