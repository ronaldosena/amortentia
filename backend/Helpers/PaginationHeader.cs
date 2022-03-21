namespace backend.Helpers
{
  public class PaginationHeader
  {
    public int CurrentPage { get; set; }
    public int ItemsPerPage { get; set; }
    public long TotalItems { get; set; }
    public int TotalPages { get; set; }

    public PaginationHeader(int currentPage, int itemsPerPage, long totalItems, int totalPages)
    {
      this.CurrentPage = currentPage;
      this.ItemsPerPage = itemsPerPage;
      this.TotalItems = totalItems;
      this.TotalPages = totalPages;
    }
  }
}