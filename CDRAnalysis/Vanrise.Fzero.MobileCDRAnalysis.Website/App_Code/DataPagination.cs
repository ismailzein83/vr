using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DataPagination
/// </summary>

[Serializable]
public class DataPagination
{
    #region Properties
    public int StartRecordNumber { get; set; }
    public int RecordsCount { get; set; }
    public int EndRecordNumber { get; set; }
    public int CurrentPage { get; set; }
    public int PagesCount { get; set; }
    public int PageSize { get; set; }
    #endregion

    public void FillDataPagination(int recordsCount, int pageSize, int currentPage ){

        RecordsCount = recordsCount;
        PageSize = pageSize;
        PagesCount = recordsCount / pageSize;
        if (recordsCount % pageSize != 0)
            PagesCount += 1;
        if (currentPage > PagesCount)
            CurrentPage = PagesCount;
        else
            CurrentPage = currentPage;
        StartRecordNumber = (CurrentPage - 1) * pageSize + 1;
        EndRecordNumber = Math.Min(StartRecordNumber + PageSize - 1, RecordsCount);
    }

    public void ChangeCurrentPage(int currentPage)
    {
        CurrentPage = currentPage;
        StartRecordNumber = (currentPage - 1) * PageSize + 1;
        EndRecordNumber = Math.Min(StartRecordNumber+ PageSize, RecordsCount);
    }

    public void ChangePageSize(int pageSize)
    {
        PageSize = pageSize;
        PagesCount = RecordsCount / pageSize;
        if (RecordsCount % pageSize == 0)
            PagesCount += 1;
        if (CurrentPage > PagesCount)
            CurrentPage = PagesCount;
        StartRecordNumber = (CurrentPage - 1) * PageSize + 1;
        EndRecordNumber = Math.Min(StartRecordNumber + PageSize, RecordsCount);
    }

    public DataPagination()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public DataPagination(int recordsCount, int pageSize, int currentpage)
    {
        FillDataPagination(recordsCount, pageSize, currentpage);
    }
}