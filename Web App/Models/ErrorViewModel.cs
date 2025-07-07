// ViewModel for displaying error information to the user
namespace Web_App.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }  // Unique ID for tracing the request

        // Indicates whether to show the RequestId in the UI
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
