namespace Clinic.API.API.Dtos.PagingDtos
{
    public class PagingDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
    }
}
