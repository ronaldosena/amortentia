using System;
using System.Collections.Generic;

namespace backend.Helpers
{
  public class PagedList<T> : List<T>
  {
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }

    public PagedList(List<T> items, long count, int pageNumber, int pageSize)
    {
      TotalCount = count;
      PageSize = pageSize;
      CurrentPage = pageNumber;
      TotalPages = (int)Math.Ceiling(count / (double)pageSize);
      this.AddRange(items);
    }
  }
}