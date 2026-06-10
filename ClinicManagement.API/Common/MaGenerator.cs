namespace ClinicManagement.API.Common;

public static class MaGenerator
{
    /// <summary>Trích phần số trong mã, ví dụ "BN0012" -> 12, "PK0003" -> 3.</summary>
    public static int ExtractNumber(string? code)
    {
        if (string.IsNullOrEmpty(code)) return 0;
        var digits = new string(code.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out var n) ? n : 0;
    }

    /// <summary>Sinh mã kế tiếp dạng PREFIX + số 4 chữ số, ví dụ ("BN", 12) -> "BN0013".</summary>
    public static string Next(string prefix, IEnumerable<string?> existingCodes)
    {
        var max = 0;
        foreach (var c in existingCodes)
        {
            var n = ExtractNumber(c);
            if (n > max) max = n;
        }
        return $"{prefix}{max + 1:D4}";
    }
}
