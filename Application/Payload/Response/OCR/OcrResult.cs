namespace Application.Payload.Response.OCR
{
    public class OcrResult
    {
        public bool IsSuccess { get; set; }
        public string Text { get; set; } = string.Empty;
        public float Confidence { get; set; }
    }
}
