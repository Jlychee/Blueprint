namespace Client.Models.Models.DTO;

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; } // Всего проектов (после фильтрации если она была)
    public int Page { get; set; } // Номер текущей страницы
    public int PageSize { get; set; } // Сколько элементов на странице
    
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize); // Сколько всего страниц
}