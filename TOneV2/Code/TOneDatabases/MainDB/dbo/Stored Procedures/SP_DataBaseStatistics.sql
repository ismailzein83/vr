-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_DataBaseStatistics]

AS
BEGIN

	SET NOCOUNT ON;

  -- Get Top 20 executed SP's ordered by logical reads (memory pressure)
    SELECT TOP 100 qt.text AS 'SP Name', total_logical_reads, 
    qs.total_worker_time AS 'TotalWorkerTime', 
    qs.total_worker_time/qs.execution_count AS 'AvgWorkerTime',
    qs.execution_count AS 'Execution Count', total_logical_reads/qs.execution_count AS 'AvgLogicalReads',
    qs.total_physical_reads, qs.total_physical_reads/qs.execution_count AS 'Avg Physical Reads',
    qs.total_logical_writes/DATEDIFF(Minute, qs.creation_time, GetDate()) AS 'Logical Writes/Min',  
    qs.total_logical_writes, qs.total_logical_writes/qs.execution_count AS 'AvgLogicalWrites',
    qs.execution_count/DATEDIFF(Second, qs.creation_time, GetDate()) AS 'Calls/Second', 
    qs.total_worker_time/qs.execution_count AS 'AvgWorkerTime',
    qs.total_worker_time AS 'TotalWorkerTime',
    qs.total_elapsed_time/qs.execution_count AS 'AvgElapsedTime',
    qs.total_logical_writes,
    qs.max_logical_reads, qs.max_logical_writes, qs.total_physical_reads, 
    DATEDIFF(Minute, qs.creation_time, GetDate()) AS 'Age in Cache', qt.dbid 
    FROM sys.dm_exec_query_stats AS qs
    CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) AS qt
    WHERE qt.dbid = db_id() -- Filter by current database
    ORDER BY total_logical_reads DESC
    
    
-- SELECT '%signal (cpu) waits' = CAST(100.0 * SUM(signal_wait_time_ms) / SUM (wait_time_ms) AS NUMERIC(20,2)),
--           '%resource waits'= CAST(100.0 * SUM(wait_time_ms - signal_wait_time_ms) / SUM (wait_time_ms) AS NUMERIC(20,2))
--      FROM sys.dm_os_wait_stats;

END