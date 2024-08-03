using System.Diagnostics.CodeAnalysis;

namespace TRENDX_ToDoList_Dennys.API.Entities
{
    [ExcludeFromCodeCoverage]
    public class RetornoPadraoDto
    {
        public bool HasError { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }
    }
}
