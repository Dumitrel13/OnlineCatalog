namespace OnlineCatalog.Helpers
{
    public static class PasswordGenerator
    {
        public static string GeneratePassword()
        {
            const int passwordLength = 10;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            const string specialChars = "!@#$%^&*()_-+={}[]|\\:;\"'<>,.?/~`";
            const string digits = "0123456789";

            var random = new Random();

            var password = new string(Enumerable.Repeat(chars, passwordLength).Select(s => s[random.Next(s.Length)])
                .ToArray());
            password += specialChars[random.Next(specialChars.Length)];
            password += digits[random.Next(digits.Length)];

            return password;
        }
    }
}
