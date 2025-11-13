namespace Transferencia.Settings
{
    public class Api
    {
        public string BaseUrl { get; set; } = string.Empty;

        public Endpoints Endpoints { get; set; } = new();
    }

    public class Endpoints
    {
        public string AdicionarMovimento { get; set; } = string.Empty;
    }
}


