using System.Text.Json.Serialization;

namespace Riaya.Application.Features.Payments.DTOs;

public class PaymobCallbackDto
{
    [JsonPropertyName("obj")]
    public PaymobTransactionObj? Obj { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public class PaymobTransactionObj
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("amount_cents")]
    public int AmountCents { get; set; }

    [JsonPropertyName("order")]
    public PaymobOrder? Order { get; set; }

    [JsonPropertyName("hmac")]
    public string? Hmac { get; set; }
}

public class PaymobOrder
{
    [JsonPropertyName("merchant_order_id")]
    public string MerchantOrderId { get; set; } = string.Empty;
}
